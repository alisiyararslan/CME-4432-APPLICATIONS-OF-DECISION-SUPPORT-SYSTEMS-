using sun.security.jca;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        string new_file;
        string temp_new_file;

        static weka.classifiers.Classifier cl_BEST = null;

        static weka.classifiers.Classifier cl_1NN = null;
        static weka.classifiers.Classifier cl_3NN = null;
        static weka.classifiers.Classifier cl_5NN = null;
        static weka.classifiers.Classifier cl_7NN = null;
        static weka.classifiers.Classifier cl_9NN = null;
        static weka.classifiers.Classifier cl_NB = null;
        static weka.classifiers.Classifier cl_LOG = null;
        static weka.classifiers.Classifier cl_J48 = null;
        static weka.classifiers.Classifier cl_RT = null;
        static weka.classifiers.Classifier cl_RF = null;
        static weka.classifiers.Classifier cl_MLP = null;
        static weka.classifiers.Classifier cl_SVM = null;

        Dictionary<weka.classifiers.Classifier, string> modelMap = new Dictionary<weka.classifiers.Classifier, string>();

        int num_att;
        List<Control> controls;
        weka.core.Instance newInstance;
        weka.core.Instances insts;
        weka.core.Instances insts_for_input;
        Label outputLabel;
        public Form1()
        {
            InitializeComponent();

        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            new_file = "";
            int controlsLen = this.Controls.Count;
            for (int i = controlsLen - 1; i >= 0; i--)
            {
                if (this.Controls[i].Name != "browseButton" && this.Controls[i].Name != "fileTextBox")
                {
                    if (this.Controls[i] is TextBox || this.Controls[i] is ComboBox)
                    {
                        this.Controls.RemoveAt(i);
                    }
                    else if(this.Controls[i].Name != "resultLabel")
                    {
                        this.Controls.Remove(this.Controls[i]);
                    }

                }

            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select File";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "All files (*.*)|*.*|Text File (*.txt)|*.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                fileTextBox.Text = ofd.FileName;
            }
            else
            {
                fileTextBox.Text = "";
            }

            var file_name = ofd.FileName;

            insts = new weka.core.Instances(new java.io.FileReader(file_name)); 
            insts_for_input = insts;
            weka.core.Instances insts_temp = new weka.core.Instances(new java.io.FileReader(file_name));

            num_att = insts_temp.numAttributes();

            new_file = "@RELATION new_instance\n";

            for (int i = 0; i < num_att; i++)
            {
                new_file += "@ATTRIBUTE " + insts_temp.attribute(i).name() + " ";
                if (insts_temp.attribute(i).isNumeric())
                {
                    new_file += "REAL\n";
                }
                else
                {
                    new_file += "{";
                    int sub_types_num = insts_temp.attribute(i).numValues();

                    for (int j = 0; j < sub_types_num; j++)
                    {
                        new_file += "'" + insts_temp.attribute(i).value(j) + "',";
                    }

                    new_file = new_file.Substring(0, new_file.Length - 1) + "}\n";
                }
            }

            new_file += "@DATA\n";

            double max_acc = ClassTest_1NN(insts);
            cl_BEST = cl_1NN;

            insts = insts_temp;
            double temp_acc = ClassTest_3NN(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_3NN;
            }

            insts = insts_temp;
            temp_acc = ClassTest_5NN(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_5NN;
            }

            insts = insts_temp;
            temp_acc = ClassTest_7NN(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_7NN;
            }

            insts = insts_temp;
            temp_acc = ClassTest_9NN(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_9NN;
            }

            insts = insts_temp;
            temp_acc = ClassTest_NB(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_NB;
            }

            insts = insts_temp;
            temp_acc = ClassTest_J48(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_J48;
            }

            insts = insts_temp;
            temp_acc = ClassTest_RT(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_RT;
            }

            insts = insts_temp;
            temp_acc = ClassTest_RF(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_RF;
            }

            insts = insts_temp;
            temp_acc = ClassTest_MLP(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_MLP;
            }

            insts = insts_temp;
            temp_acc = ClassTest_SVM(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_SVM;
            }

            insts = insts_temp;
            temp_acc = ClassTest_LOG(insts);

            if (temp_acc > max_acc)
            {
                max_acc = temp_acc;
                cl_BEST = cl_LOG;
            }
            string model_file = "";
            model_file = file_name.Replace(".arff", "_cl.model");

            weka.core.SerializationHelper.write(model_file, cl_BEST);

            modelMap.Add(cl_1NN, "1 Nearest Neighbour");
            modelMap.Add(cl_3NN, "3 Nearest Neighbour");
            modelMap.Add(cl_5NN, "5 Nearest Neighbour");
            modelMap.Add(cl_7NN, "7 Nearest Neighbour");
            modelMap.Add(cl_9NN, "9 Nearest Neighbour");
            modelMap.Add(cl_NB, "Naive Bayes");
            modelMap.Add(cl_LOG, "Logistic Regression");
            modelMap.Add(cl_J48, "J48");
            modelMap.Add(cl_RT, "Random Trees");
            modelMap.Add(cl_RF, "Random Forest");
            modelMap.Add(cl_MLP, "Multilayer Perceptron");
            modelMap.Add(cl_SVM, "Support Vector Machine");

            this.resultLabel.Text = $"{modelMap[cl_BEST]} is the most successful algorithm for this data ({max_acc})";

            newInstance = new weka.core.Instance(insts.numAttributes());
            newInstance.setDataset(insts);

            controls = new List<Control>();

            for (int i = 0; i < num_att - 1; i++)
            {
                string attributeName = insts_temp.attribute(i).name();
                Label label = new Label();
                label.Text = $"{attributeName}: ";
                label.Location = new Point(35, 80 + i * 25);
                this.Controls.Add(label);

                weka.core.AttributeStats stats = insts_temp.attributeStats(i);
                int uniqueCount = stats.distinctCount;

                if (uniqueCount < 13)//categorical
                {
                    ComboBox comboBox = new ComboBox();
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    HashSet<string> uniqueValues = new HashSet<string>();
                    for (int j = 0; j < insts_temp.numInstances(); j++)
                    {

                        string value;
                        if (insts_temp.attribute(i).isNominal() || insts_temp.attribute(i).isString())
                        {
                            value = insts_temp.instance(j).stringValue(i);
                        }
                        else if (insts_temp.attribute(i).isNumeric())
                        {
                            // Handle numeric attribute value (optional)
                            value = insts_temp.instance(j).value(i).ToString();
                        }
                        else if (insts_temp.attribute(i).isDate())
                        {
                            // Handle date attribute value (optional)
                            value = insts_temp.instance(j).stringValue(i); // or use appropriate method to format date
                        }
                        else
                        {
                            // Handle boolean attribute value
                            value = insts_temp.instance(j).value(i) == 0 ? "false" : "true"; // Convert boolean value to string
                        }
                        uniqueValues.Add(value);
                    }

                    string[] nominalValues = uniqueValues.ToArray();

                    comboBox.Items.AddRange(nominalValues);
                    comboBox.Location = new Point(180, 80 + i * 25);
                    this.Controls.Add(comboBox);

                    controls.Add(comboBox);
                }
                else
                {//numerical
                    TextBox textBox = new TextBox();
                    textBox.Location = new Point(180, 80 + i * 25);
                    this.Controls.Add(textBox);

                    controls.Add(textBox);
                }
            }

            Button button = new Button();
            button.Text = "Discover";
            button.Location = new Point(32, 80 + num_att * 25); // Adjust the location as needed
            button.Click += Discover_Button_Click;
            this.Controls.Add(button);

            outputLabel = new Label();
            outputLabel.AutoSize = true; // Add this line
            outputLabel.Text = "";
            outputLabel.Location = new Point(32, 80 + (num_att+1) * 25); // Adjust these values as needed
            this.Controls.Add(outputLabel);

            StreamWriter sw_new_inst = new StreamWriter("new_instance.arff");
            sw_new_inst.Write(new_file);
            sw_new_inst.Close();

            temp_new_file = new_file;
        }

        private void Discover_Button_Click(object sender, EventArgs e)
        {
            new_file = temp_new_file;
            new_file = new_file.Replace("'", "");
            for (int i = 0; i < num_att - 1; i++)
            {
                new_file += controls[i].Text + ",";

            }
            new_file += "?";



            for (int i = 0; i < insts_for_input.numInstances(); i++)
            {
                for (int j = 0; j < insts_for_input.numAttributes(); j++)
                {
                    weka.core.Attribute attribute = insts_for_input.attribute(j);
                    if (j == 0)
                    {
                        new_file += "\n";
                    }

                    if (attribute.isNominal())
                    {
                        new_file += insts_for_input.instance(i).stringValue(j);
                    }
                    else
                    {
                        new_file += insts_for_input.instance(i).value(j).ToString().Replace(',', '.');
                    }

                    if (j != insts_for_input.numAttributes() - 1)
                    {
                        new_file += ",";
                    }

                }
            }

            StreamWriter sw_new_inst = new StreamWriter("new_instance.arff");
            sw_new_inst.Write(new_file);
            sw_new_inst.Close();

            weka.core.Instances new_insts = new weka.core.Instances(new java.io.FileReader("new_instance.arff"));
            new_insts.setClassIndex(new_insts.numAttributes() - 1);

            if (cl_BEST.GetType().FullName.ToString() == "weka.classifiers.lazy.IBk" ||
                cl_BEST.GetType().FullName.ToString() == "weka.classifiers.functions.MultilayerPerceptron" ||
                cl_BEST.GetType().FullName.ToString() == "weka.classifiers.functions.SMO" ||
                cl_BEST.GetType().FullName.ToString() == "weka.classifiers.functions.Logistic")
            {
                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(new_insts);
                new_insts = weka.filters.Filter.useFilter(new_insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(new_insts);
                new_insts = weka.filters.Filter.useFilter(new_insts, myDummyAttr);
            }
            else if (cl_BEST.GetType().FullName.ToString() == "weka.classifiers.bayes.NaiveBayes")
            {
                weka.filters.Filter myDiscretize = new weka.filters.unsupervised.attribute.Discretize();
                myDiscretize.setInputFormat(new_insts);
                new_insts = weka.filters.Filter.useFilter(new_insts, myDiscretize);
            }

            double predictedClass = cl_BEST.classifyInstance(new_insts.instance(0));

            outputLabel.Text = $"RESULT: {new_insts.classAttribute().value(Convert.ToInt32(predictedClass))}";
        }

        public static double ClassTest_1NN(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);
                cl_1NN = new weka.classifiers.lazy.IBk(1);
                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);
                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_1NN.buildClassifier(insts);

                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_1NN, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_3NN(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_3NN = new weka.classifiers.lazy.IBk(3);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_3NN.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_3NN, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_5NN(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_5NN = new weka.classifiers.lazy.IBk(5);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_5NN.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_5NN, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_7NN(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_7NN = new weka.classifiers.lazy.IBk(7);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_7NN.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_7NN, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_9NN(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_9NN = new weka.classifiers.lazy.IBk(9);

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_9NN.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_9NN, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_NB(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_NB = new weka.classifiers.bayes.NaiveBayes();

                //discretize
                weka.filters.Filter myDiscretize = new weka.filters.unsupervised.attribute.Discretize();
                myDiscretize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDiscretize);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_NB.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_NB, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_LOG(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_LOG = new weka.classifiers.functions.Logistic();

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_LOG.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_LOG, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_J48(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_J48 = new weka.classifiers.trees.J48();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_J48.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_J48, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_RF(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_RF = new weka.classifiers.trees.RandomForest();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_RF.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_RF, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_RT(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_RT = new weka.classifiers.trees.RandomTree();

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_RT.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_RT, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

        public static double ClassTest_MLP(weka.core.Instances insts)
        {
            Application.DoEvents();
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_MLP = new weka.classifiers.functions.MultilayerPerceptron();

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_MLP.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_MLP, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
            
        }

        public static double ClassTest_SVM(weka.core.Instances insts)
        {
            try
            {
                insts.setClassIndex(insts.numAttributes() - 1);

                cl_SVM = new weka.classifiers.functions.SMO();

                weka.filters.Filter myNormalize = new weka.filters.unsupervised.attribute.Normalize();
                myNormalize.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myNormalize);

                weka.filters.Filter myDummyAttr = new weka.filters.unsupervised.attribute.NominalToBinary();
                myDummyAttr.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myDummyAttr);

                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                cl_SVM.buildClassifier(insts);
                //10 fold cross validation
                weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
                eval.crossValidateModel(cl_SVM, insts, 10, new java.util.Random(1));

                double accuracy = eval.pctCorrect();
                return accuracy;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return 0;
            }
        }

    }
}
