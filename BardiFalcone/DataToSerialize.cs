using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace BardiFalcone
{
    [Serializable]
    public class DataToSerialize
    {
        public int iters;
        public double timeStep;
        public int currentIter;
        public int saveIter;
        public string savePath;
        public string foldername;
        public int dim;
        public double[] sizeOfBox;
                
        public SortedDictionary<Point, Value> pointsFirst;        
        public SortedDictionary<Point, Value> pointsSecond;
    }
}
