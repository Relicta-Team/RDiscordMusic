using LibNoise;
using LibNoise.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ReCaves
{
    public class Generator
    {
        public int width = (int)Math.Abs(GeneratorConfig.Instance.generationZone.endPos.X - GeneratorConfig.Instance.generationZone.startPos.X);
        public int height = (int)Math.Abs(GeneratorConfig.Instance.generationZone.endPos.Y - GeneratorConfig.Instance.generationZone.startPos.Y);

        //TODO переписать эти поля в конфиг, что бы была возможность удобно настраивать через опции
        public double threshold { get; set; } = 0.5; // тоже стоит поиграться
        public double scale { get; set; } = 0.1; // играемся между 1 и 0
        public int seed { get; set; }
        public NoiseQuality noiseQuality { get; set; } = NoiseQuality.Fast;

        public bool[,] CaveMap { get; set; }
        public Generator(double threshold, double scale)
        {
            this.threshold = threshold;
            this.scale = scale;
            CaveMap = new bool[width, height];
            seed = new Random().Next();
        }

        public void GenerateCave()
        {
            ImprovedPerlin perlin = new ImprovedPerlin(seed, noiseQuality);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double noiseVal = (float)perlin.GetValue((float)(x * scale), (float)(y * scale), 0);
                    CaveMap[x, y] = noiseVal < threshold;
                }
            }
        }

        public void PrintMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Console.Write(CaveMap[x, y] ? " " : "#");
                }
                Console.WriteLine();
            }
        }
    }
}
