using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReCaves
{
    public class GeneratorConfig
    {
        private static GeneratorConfig instance;
        public static GeneratorConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GeneratorConfig(new GenerationZone(new Point2D { X = 0, Y = 0 }, new Point2D { X = 0, Y = 0 }));
                }
                return instance;
            }
        }

        public uint Levels { get; set; } = 1;       // Кол-во этажей, по умолчанию 1
        public uint Transitions { get; set; } = 0;  // Кол-во переходов, 0 - временное решение
        public uint ChunkSize { get; set; } = 10;  // По умолчанию 10

        public List<Zone> zonesOfInterest = new List<Zone>();           // Список Пустот
        public List<Point3D> hardTransitions = new List<Point3D>();     // Список Переходов
        public List<Point3D> exits = new List<Point3D>();               // Список Выходов
        public GenerationZone generationZone;
        public GeneratorConfig(GenerationZone gz)   // Прокидываем обязательную зону для генерации
        {
            generationZone = gz;
        }

        public struct Zone                                      // Зоны пустот
        {
            public Zone(Point3D pos, float rad)
            {
                position = pos;
                radius = rad;
            }
            public Point3D position;
            public float radius;
        }

        public struct GenerationZone                           // Зона для генерации
        {
            public GenerationZone(Point2D sPos, Point2D ePos)  // Прокидываем позиции
            {
                startPos = sPos;
                endPos = ePos;
            }
            public Point2D startPos;
            public Point2D endPos;
        }
    }
}
