using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DSSWindowsFormsAppSOM
{
    public partial class Form1 : Form
    {
        string new_file;
        string file_name;
        int lng;
        Map map = new Map();
        public Form1()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            new_file = "";
            int controlsLen = this.Controls.Count;


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select File";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                dataSetTextBox.Text = ofd.FileName;
            }
            else
            {
                dataSetTextBox.Text = "";
            }

            file_name = ofd.FileName;

            StreamReader sr = new StreamReader(file_name);
            lng = sr.ReadLine().Split(',').Length;
        }

        private void somButton_Click(object sender, EventArgs e)
        {
            
            int length = Int32.Parse(lengthTextBox.Text);
            map = new Map(lng - 1, length, file_name);


            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                    button.Text = $"{j},{i}";
                    button.Location = new Point(32+ i*40, 150 + j * 40); // Adjust the location as needed
                    button.Click += Coordiante_Button_Click;
                    button.Size = new Size(35, 35);
                    this.Controls.Add(button);
                }
            }
        }
        private void Coordiante_Button_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;

            // Check if the cast was successful
            if (clickedButton != null)
            {
                // Access the Text property of the clicked button
                string buttonText = clickedButton.Text;

                string[] buttonTextArray = buttonText.Split(',');

                Dictionary<int, string> fileDict = new Dictionary<int, string>();

                using (StreamReader sr = new StreamReader(file_name))
                {
                    int currentRow = 1;

                    // Loop through the file until reaching the desired row (or end of file)
                    while (true)
                    {
                        string line = sr.ReadLine();

                        // Check if the line is null (end of file)
                        if (line == null)
                        {
                            
                            break; // Exit the method or handle the error accordingly
                        }
                        fileDict.Add(currentRow, line);
                        currentRow++;
                    }
                }

                Coordinate key = new Coordinate { X = Int32.Parse(buttonTextArray[0]), Y = Int32.Parse(buttonTextArray[1]) };

                int x = Int32.Parse(buttonTextArray[0]);

                int y = Int32.Parse(buttonTextArray[1]);

                List<string> somlist = new List<string>();

                somlist.Add(fileDict[1]);

                

                foreach (KeyValuePair<Coordinate, int> ele in map.coordinates)
                {
                    if (ele.Key.X == x && ele.Key.Y == y)
                    {
                        somlist.Add(fileDict[ele.Value+2]);
                    }
                }

                string form2Title = $"Instances in {buttonText}";
                Form2 form = new Form2(somlist, form2Title);
                form.Show();
            }
        }
        
    }

    

    public class Map
    {
        private Neuron[,] outputs;  // Collection of weights.
        private int iteration;      // Current iteration.
        private int length;        // Side length of output grid.
        private int dimensions;    // Number of input dimensions.
        private Random rnd = new Random();

        private List<string> labels = new List<string>();
        private List<double[]> patterns = new List<double[]>();

        private List<string> patterns_with_target_attr = new List<string>();

        public Dictionary<Coordinate, int> coordinates = new Dictionary<Coordinate, int>();

        public Map()
        {

        }
        public Map(int dimensions, int length, string file)
        {
            this.length = length;
            this.dimensions = dimensions;
            Initialise();
            LoadData(file);
            NormalisePatterns();
            Train(0.0000001);
            DumpCoordinates();

            double sse = 0;
            for (int i = 0; i < outputs.GetLength(0); i++)
            {
                for (int j = 0; j < outputs.GetLength(1); j++)
                {
                    sse += outputs[i, j].sse1();
                }
            }

            Console.WriteLine(sse);
            Console.ReadLine();
        }

        private void Initialise()
        {
            outputs = new Neuron[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    outputs[i, j] = new Neuron(i, j, length);
                    outputs[i, j].Weights = new double[dimensions];
                    outputs[i, j].total_attr_values = new double[dimensions];
                    for (int k = 0; k < dimensions; k++)
                    {
                        outputs[i, j].Weights[k] = rnd.NextDouble();
                    }
                }
            }
        }

        private void LoadData(string file)
        {
            StreamReader reader = File.OpenText(file);
            reader.ReadLine(); // Ignore first line.
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                labels.Add(line[0]);
                double[] inputs = new double[dimensions];
                for (int i = 0; i < dimensions; i++)
                {
                    inputs[i] = double.Parse(line[i]);

                }
                patterns.Add(inputs);
                patterns_with_target_attr.Add(line[dimensions]);
            }
            reader.Close();
        }

        private void NormalisePatterns()
        {
            for (int j = 0; j < dimensions; j++)
            {
                double max = 0;

                for (int i = 0; i < patterns.Count; i++)
                {
                    if (patterns[i][j] > max) max = patterns[i][j];
                }
                for (int i = 0; i < patterns.Count; i++)
                {
                    patterns[i][j] = patterns[i][j] / max;
                }
            }
        }

        private void Train(double maxError)
        {
            double currentError = double.MaxValue;
            while (currentError > maxError)
            {
                currentError = 0;
                List<double[]> TrainingSet = new List<double[]>();
                foreach (double[] pattern in patterns)
                {
                    TrainingSet.Add(pattern);
                }
                for (int i = 0; i < patterns.Count; i++)
                {
                    double[] pattern = TrainingSet[rnd.Next(patterns.Count - i)];
                    currentError += TrainPattern(pattern);
                    TrainingSet.Remove(pattern);
                }
                Console.WriteLine(currentError.ToString("0.0000000"));
            }
        }

        private double TrainPattern(double[] pattern)
        {
            double error = 0;
            Neuron winner = Winner(pattern);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    error += outputs[i, j].UpdateWeights(pattern, winner, iteration);
                }
            }
            iteration++;
            return Math.Abs(error / (length * length));
        }

        private void DumpCoordinates()
        {
            
            for (int i = 0; i < patterns.Count; i++)
            {
                Neuron n = Winner(patterns[i]);
                n.data.Add(patterns[i]);
                n.tr.Add(patterns_with_target_attr[i]);

                for (int j = 0; j < patterns[i].Length; j++)
                    n.total_attr_values[j] += patterns[i][j];

                Console.WriteLine("{0},{1},{2}", i, n.X, n.Y);

                coordinates[new Coordinate { X = n.X, Y = n.Y }] = i;
            }
        }

        private Neuron Winner(double[] pattern)
        {
            Neuron winner = null;
            double min = double.MaxValue;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    double d = Distance(pattern, outputs[i, j].Weights);
                    if (d < min)
                    {
                        min = d;
                        winner = outputs[i, j];
                    }
                }
            return winner;
        }

        private double Distance(double[] vector1, double[] vector2)
        {
            double value = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                value += Math.Pow((vector1[i] - vector2[i]), 2);
            }
            return Math.Sqrt(value);
        }
    }

    public class Neuron
    {
        public double[] Weights;
        public int X;
        public int Y;
        private int length;
        private double nf;
        public List<double[]> data;
        public List<string> tr;

        public double sse1()
        {
            double retV = 0;
            for (int i = 0; i < data.Count; i++)
            {
                retV += Distance(Weights, data[i]);
            }
            return retV;
        }

        private double Distance(double[] vector1, double[] vector2)
        {
            double value = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                value += Math.Pow((vector1[i] - vector2[i]), 2);
            }
            return value;
        }


        public double[] total_attr_values;



        public Neuron(int x, int y, int length)
        {
            X = x;
            Y = y;
            this.length = length;
            nf = 1000 / Math.Log(length);
            data = new List<double[]>();
            tr = new List<string>();
        }

        private double Gauss(Neuron win, int it)
        {
            double distance = Math.Sqrt(Math.Pow(win.X - X, 2) + Math.Pow(win.Y - Y, 2));
            return Math.Exp(-Math.Pow(distance, 2) / (Math.Pow(Strength(it), 2)));
        }

        private double LearningRate(int it)
        {
            return Math.Exp(-it / 1000) * 0.1;
        }

        private double Strength(int it)
        {
            return Math.Exp(-it / nf) * length;
        }

        public double UpdateWeights(double[] pattern, Neuron winner, int it)
        {
            double sum = 0;
            for (int i = 0; i < Weights.Length; i++)
            {
                double delta = LearningRate(it) * Gauss(winner, it) * (pattern[i] - Weights[i]);
                Weights[i] += delta;
                sum += delta;
            }
            return sum / Weights.Length;
        }
    }
}
