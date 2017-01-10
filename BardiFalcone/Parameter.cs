using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BardiFalcone
{
    /// <summary>
    ///  Структура для параметров решетки, считываемых из файла
    ///  </summary>
    [Serializable]
    public struct Parameter
    {   
        /// <summary>
        /// Шаг по пространству
        /// </summary>
        public double[] SizeOfBox { get; set; }        

        /// <summary>
        /// Шаг по времени
        /// </summary>
        public double TimeStep { get; set; }
        
        /// <summary>
        /// Количество итераций
        /// </summary>
        public int Iter { get; set; }
        
        /// <summary>
        ///  Количество итераций, через которое необходимо сохранять состояние программы
        /// </summary>
        public int SaveIter { get; set; }

        /// <summary>
        /// Размер области
        /// </summary>
        public double[] SizeOfDomain { get; set; }

        /// <summary>
        /// Начальная точка
        /// </summary>
        public Point InitialPoint { get; set; }

        /// <summary>
        /// Название директории, в которую сохраняются результаты подсчета
        /// </summary>
        public string FolderName { get; set; }
    }
}
