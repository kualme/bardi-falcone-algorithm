using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using BardiFalcone.TerminalSet;
using BardiFalcone.FailureSet;

namespace BardiFalcone
{
    class Compute
    {
        private Grid _grid;
        private Function.function _function;
        private IControlSet _controlSet1;
        private IControlSet _controlSet2;
        private ITerminalSet _terminalSet;
        private IFailureSet _failureSet;
        
        private int _iters;
        private double _timeStep;
                        
        private string _foldername;

        private Dictionary<Point, Point> controls1;
        private Dictionary<Point, Point> controls2;
        private Dictionary<Point, Point> controlsSum;

        private const double INF = 1E10;
        private const double SUP = 1E38;
        
        private int _saveIter;
        private string _savePath;
        private DataToSerialize _data;

        private TimeSpan _tick;

        public Compute(Grid grid, Function.function function, IControlSet controlSet1, IControlSet controlSet2, ITerminalSet terminalSet, int iters, double timeStep, int saveIter, IFailureSet failureSet = null, string folderName = null)
        {   
            _grid = grid;
            _function = function;
            _controlSet1 = controlSet1;
            _controlSet2 = controlSet2;
            _terminalSet = terminalSet;
            _failureSet = failureSet == null ? new EmptyFailureSet() : failureSet;
            _iters = iters;
            _timeStep = timeStep;
            _saveIter = saveIter;

            _foldername = folderName == null ? String.Format("gridStep-{0}_timeStep-{1}_{2}", grid.sizeOfBox[0], timeStep, DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")) : folderName;
            System.IO.Directory.CreateDirectory(_foldername);
            _savePath = Path.Combine(_foldername, "saves");
            System.IO.Directory.CreateDirectory(_savePath);

            // Trace
            Trace.Listeners.Clear();

            TextWriterTraceListener twtl = new TextWriterTraceListener(Path.Combine(_foldername, "log.txt"));
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;

            Trace.WriteLine("Time: " + DateTime.Now.TimeOfDay);
            Trace.WriteLine("Time Step: " + _timeStep + ", Grid Step: " + _grid.sizeOfBox[0]);
            Trace.WriteLine("Quantity of points in grid: " + _grid.points[0].Count);
            Trace.WriteLine("|Control Set 1| = " + _controlSet1.Set.Length);
            Trace.WriteLine("|Control Set 2| = " + _controlSet2.Set.Length);
            
            controls1 = new Dictionary<Point, Point>();
            controls2 = new Dictionary<Point, Point>();
            controlsSum = new Dictionary<Point, Point>();

            _data = new DataToSerialize();
            _data.dim = _grid.dim;
            _data.foldername = _foldername;
            _data.iters = _iters;
            _data.saveIter = _saveIter;
            _data.savePath = _savePath;
            _data.sizeOfBox = grid.sizeOfBox;

            _tick = DateTime.Now.TimeOfDay;

        }

        public void Work()
        {
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            Iterate(0);
            
            totalStopwatch.Stop();
            TimeSpan totalTime = totalStopwatch.Elapsed;
            String elapsedTotalTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", totalTime.Hours, totalTime.Minutes, totalTime.Seconds, totalTime.Milliseconds / 10);
            Trace.WriteLine("Total Run Time: " + elapsedTotalTime);
        }

        private void Iterate(int firstIter)
        {
            double gamma = Math.Exp(-_timeStep);
            TimeSpan iterTime = new TimeSpan();

            for (int iter = firstIter; iter < _iters; ++iter)
            {
                Stopwatch iterStopwatch = new Stopwatch();
                iterStopwatch.Start();
                
#if DEBUG
                controls1.Clear();
                controls2.Clear();
                controlsSum.Clear();
#endif

                int pointId = 0;
                int pointP = 0;
                // thisIdx = 0, eсли iter четный, и = 1, если iter нечетный.
                int thisIdx = iter % 2;
                // otherIdx = 1, если iter четный, и = 0, если iter нечетный 
                int otherIdx = 1 - thisIdx;

                try
                {
                    Parallel.ForEach(_grid._keys, point =>
                    //foreach (Point point in _grid.points[thisIdx].Keys)
                    {
                        if (_grid.points[thisIdx][point].isTerminalPoint)
                        {
                            _grid.points[otherIdx][point] = _grid.points[thisIdx][point];
                            ++pointId;
                            if (Math.Floor((pointId * 100.0) / _grid.points[0].Count) > pointP)
                            {
                                pointP = (int)Math.Floor((pointId * 100.0) / _grid.points[0].Count);
                                if (pointP % 10 == 0)
                                    Trace.WriteLine(String.Format("Time: {0}, {1}%", DateTime.Now.TimeOfDay, pointP));
                            }
                            //continue;
                            return;
                        }
                        double inf = INF;
                        Point bestControl1 = null;
                        Point bestControl2 = null;
                        // перебираем допустимые управления первого игрока
                        foreach (Point control1 in _controlSet1.Set)
                        {
                            double sup = 0;
                            // флаг, сигнализирующий попадание новой точки в множество Q
                            bool belongsToGrid = false;

                            // перебираем допустимые управления второго игрока                        
                            foreach (Point control2 in _controlSet2.Set)
                            {
                                // метод Эйлера
                                Point newPoint = point + _function(point, control1, control2) * _timeStep;

                                double valueAtPoint = _grid.GetValueAtPoint(newPoint, thisIdx, out belongsToGrid);

                                var belongsToFailureSet = _failureSet.Belongs(newPoint);

                                if (belongsToGrid && !belongsToFailureSet && valueAtPoint > sup)
                                {
                                    sup = valueAtPoint;
                                    bestControl2 = control2;
                                }

                                // пишем что-нибудь в лог каждую минуту

                                if (DateTime.Now.TimeOfDay - _tick >= new TimeSpan(0, 1, 0))
                                {
                                    Trace.WriteLine(String.Format("Time: {0}, current point: {1}, u: {2}, v: {3}", DateTime.Now.TimeOfDay, point, control1, control2));
                                    _tick = DateTime.Now.TimeOfDay;
                                }
                            }
                            if (sup < inf && belongsToGrid)
                            {
                                inf = sup;
                                bestControl1 = control1;
                            }
                        }

#if DEBUG
                        controls2.Add(point, bestControl2);
                        controls1.Add(point, bestControl1);
                        //controlsSum.Add(point, bestControl1 + bestControl2);
#endif

                        // Записываем новое значение                    
                        double resultValue = gamma * inf + 1 - gamma;
                        if (0 <= resultValue && resultValue <= 1)
                            _grid.points[otherIdx][point] = new Value(false, resultValue);

                        ++pointId;
                        if (Math.Floor((pointId * 100.0) / _grid.points[0].Count) > pointP)
                        {
                            pointP = (int)Math.Floor((pointId * 100.0) / _grid.points[0].Count);
                            if (pointP % 10 == 0)
                                Trace.WriteLine(String.Format("Time: {0}, {1}%", DateTime.Now.TimeOfDay, pointP));
                        }
                    });
                    //}
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(String.Format("Time: {0}\n\rError: {1}\n\rStackTrace: {2}\n\rInnerException: {3}", DateTime.Now.TimeOfDay, ex.Message, ex.StackTrace, ex.InnerException));
                }

                Trace.WriteLine(String.Format("Time: {0}, Iteration finished: {1}", DateTime.Now.TimeOfDay, iter));

                PrintIterationResults(iter);
#if DEBUG
                PrintControls(iter);
#endif

                iterStopwatch.Stop();
                iterTime = iterStopwatch.Elapsed;
                String elapsedIterTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", iterTime.Hours, iterTime.Minutes, iterTime.Seconds, iterTime.Milliseconds / 10);
                Trace.WriteLine("Iteration Run Time: " + elapsedIterTime);
                Trace.WriteLine("Iteration results saved");

                // сохранение данных в файл
                if (iter % _saveIter == 0)
                {
                    _data.pointsFirst = _grid.points[0];
                    _data.pointsSecond = _grid.points[1];
                    _data.currentIter = iter;
                    Helpers.Serialize(_data.savePath, _data, iter);
                }
            }
        }
        
        private void PrintControls(int iteration)
        {            
            StreamWriter writer = new StreamWriter(Path.Combine(_foldername, "contr_1_" + iteration + ".txt"));
            foreach (Point point in _grid.points[0].Keys)
            {
                if (controls1.ContainsKey(point))
                    writer.WriteLine("{0:0.0000} {1:0.0000}", controls1[point][0], controls1[point][1]);
                else writer.WriteLine("0.0000 0.0000");
            }
            writer.Close();

            writer = new StreamWriter(Path.Combine(_foldername, "contr_2_" + iteration + ".txt"));
            foreach (Point point in _grid.points[0].Keys)
            {
                if (controls2.ContainsKey(point))
                    writer.WriteLine("{0:0.0000} {1:0.0000}", controls2[point][0], controls2[point][1]);
                else writer.WriteLine("0.0000 0.0000");
            }
            writer.Close();

            writer = new StreamWriter(Path.Combine(_foldername, "contr_sum_" + iteration + ".txt"));
            foreach (Point point in _grid.points[0].Keys)
            {
                if (controlsSum.ContainsKey(point))
                    writer.WriteLine("{0:0.0000} {1:0.0000}", controlsSum[point][0], controlsSum[point][1]);
                else writer.WriteLine("0.0000 0.0000");
            }
            writer.Close();
        }

        private void PrintIterationResults(int iteration)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(_foldername, "out" + iteration + ".txt"));
            foreach (Point point in _grid.points[0].Keys)
            {
                double val;
                if (_grid.points[iteration % 2][point].value == 1.0)
                    val = SUP;
                else 
                    val = -Math.Log(1 - _grid.points[iteration % 2][point].value);                
                //writer.WriteLine("{0:0.0000} {1:0.0000} {2:0.0000} {3}", point[0], point[1], point[2], val);
                StringBuilder sb = new StringBuilder("");
                for (int i = 0; i < point.Dim; i++)
                    sb.Append(String.Format("{0:0.0000} ", point[i]));
                sb.Append(val);
                writer.WriteLine(sb.ToString());
            }
            writer.Close();
        }

        public void Continue(string path)
        {
            DataToSerialize data = Helpers.Deserialize(path);
            _grid.points[0] = data.pointsFirst;
            _grid.points[1] = data.pointsSecond;
            _grid.sizeOfBox = data.sizeOfBox;
            _grid.dim = data.dim;

            _timeStep = data.timeStep;
            _saveIter = data.saveIter;
            _savePath = data.savePath;
            _foldername = data.foldername;

            Iterate(_data.currentIter);            
        }
    }
}
