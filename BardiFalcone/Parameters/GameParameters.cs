using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BardiFalcone.Parameters
{
    /// <summary>
    /// Структура для параметров решетки, считываемых из файла
    /// </summary>
    [Serializable]
    public struct GameParameters
    {   
        /// <summary>
        /// Количество итераций
        /// </summary>
        public int Iter { get; set; }
        
        /// <summary>
        ///  Количество итераций, через которое необходимо сохранять состояние программы
        /// </summary>
        public int SaveIter { get; set; }

        /// <summary>
        /// Название директории, в которую сохраняются результаты подсчета
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Параметры решетки
        /// </summary>
        public GridParameters GridParameters { get; set; }

        /// <summary>
        /// Параметры для инициализации множества управлений первого игрока
        /// </summary>
        public ControlSetParameter U { get; set; }

        /// <summary>
        /// Параметры для инициализации множества управлений второго игрока
        /// </summary>
        public ControlSetParameter V { get; set; }

        /// <summary>
        /// Тип функции
        /// </summary>
        public string FunctionType { get; set; }
    }
}
