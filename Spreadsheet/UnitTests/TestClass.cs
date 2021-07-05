// NAME: Hien Duong
// WSU ID: 11587750
// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using CptS321;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// test class to test the functionality of the spreadsheet engine.
    /// </summary>
    [TestFixture]
    public class TestClass
    {
        /// <summary>
        /// Test method to test the properties of the cell class.
        /// </summary>
        [Test]
        public void TestCellClass()
        {
            SpreadsheetCell testCell = new SpreadsheetCell(0, 0);

            testCell.Value = "Hello World";
            testCell.Text = "=5+2";

            Assert.AreEqual("Hello World", testCell.Value);
            Assert.AreEqual("=5+2", testCell.Text);
            Assert.AreEqual(0, testCell.RowIndex);
            Assert.AreEqual(0, testCell.ColumnIndex);
        }

        /// <summary>
        /// Test method constructs a spreadsheet and tests the functionality of the get cell method.
        /// </summary>
        [Test]
        public void TestGetCell()
        {
            Spreadsheet test = new Spreadsheet(50, 26);

            // Boundary Case, checks if you try to get a Cell not in range.
            Assert.AreEqual(null, test.GetCell(-1, -1));

            Assert.AreEqual(null, test.GetCell(100, 1000000));

            // Normal Case, gets the cell at that location in the 2d array.
            Assert.AreEqual(5, test.GetCell(5, 10).RowIndex);

            Assert.AreEqual(10, test.GetCell(5, 10).ColumnIndex);

            // Edge Case
            Assert.AreEqual(null, test.GetCell(50, 26));

            Assert.AreEqual(0, test.GetCell(0, 0).RowIndex);

            Assert.AreEqual(0, test.GetCell(0, 0).ColumnIndex);
        }

        /// <summary>
        /// Test Method to test the pulling of the values.
        /// </summary>
        [Test]
        public void TestCellPropetyChanged()
        {
            Spreadsheet test = new Spreadsheet(50, 26);

            // Set text to cell B25.
            test.GetCell(25, 1).Text = "Hello World!";

            // pull the value form cell B25.
            test.GetCell(1, 5).Text = "=B26";

            Assert.AreEqual("Hello World!", test.GetCell(1, 5).Value);

            // set text to cell A1
            test.GetCell(1, 0).Text = "LOOOOOOOOOOL OK";

            // pull the value from cell A1
            test.GetCell(30, 7).Text = "=A2";

            Assert.AreEqual("LOOOOOOOOOOL OK", test.GetCell(30, 7).Value);

            // pull the value from cell B25 already defined above
            test.GetCell(5, 25).Text = "=B26";

            // then pull the value from the cell above that is pulling from cell B25
            test.GetCell(43, 10).Text = "=Z6";

            Assert.AreEqual("Hello World!", test.GetCell(43, 10).Value);

            test.GetCell(0, 1).Text = "This is cell B1";

            test.GetCell(0, 0).Text = "=B1";

            Assert.AreEqual("This is cell B1", test.GetCell(0, 0).Value);
        }

        //-------------------- HW 5 Tests --------------------------

        /// <summary>
        /// Testing the converion of infix to postfix.
        /// </summary>
        [Test]
        public void TestPostfixConverter()
        {
            // Normal Cases
            ExpressionTree test = new ExpressionTree("X1+Y2+B3");
            MethodInfo methodInfo = this.GetMethod("ConvertToPostfix", test, new Type[] { });
            Assert.AreEqual("X1 Y2 + B3 +", methodInfo.Invoke(test, new object[] { }));

            ExpressionTree test1 = new ExpressionTree("X1-Y2-B3");
            methodInfo = this.GetMethod("ConvertToPostfix", test1, new Type[] { });
            Assert.AreEqual("X1 Y2 - B3 -", methodInfo.Invoke(test1, new object[] { }));

            ExpressionTree test2 = new ExpressionTree("X12+(Y23+B45)");
            methodInfo = this.GetMethod("ConvertToPostfix", test2, new Type[] { });
            Assert.AreEqual("X12 Y23 B45 + +", methodInfo.Invoke(test2, new object[] { }));

            ExpressionTree test3 = new ExpressionTree("Hello-World-ABCDEFGH");
            methodInfo = this.GetMethod("ConvertToPostfix", test3, new Type[] { });
            Assert.AreEqual("Hello World - ABCDEFGH -", methodInfo.Invoke(test3, new object[] { }));

            // Boundary Case
            ExpressionTree test4 = new ExpressionTree("X");
            methodInfo = this.GetMethod("ConvertToPostfix", test4, new Type[] { });
            Assert.AreEqual("X", methodInfo.Invoke(test4, new object[] { }));

            ExpressionTree test5 = new ExpressionTree("12*Y/R");
            methodInfo = this.GetMethod("ConvertToPostfix", test5, new Type[] { });
            Assert.AreEqual("12 Y * R /", methodInfo.Invoke(test5, new object[] { }));

            ExpressionTree test6 = new ExpressionTree("A+B*C");
            methodInfo = this.GetMethod("ConvertToPostfix", test6, new Type[] { });
            Assert.AreEqual("A B C * +", methodInfo.Invoke(test6, new object[] { }));

            // Edge Cases
            ExpressionTree test7 = new ExpressionTree("A+B*C/D+(F*G*H+I)");
            methodInfo = this.GetMethod("ConvertToPostfix", test7, new Type[] { });
            Assert.AreEqual("A B C * D / + F G * H * I + +", methodInfo.Invoke(test7, new object[] { }));

            ExpressionTree test8 = new ExpressionTree("A*B/C*D");
            methodInfo = this.GetMethod("ConvertToPostfix", test8, new Type[] { });
            Assert.AreEqual("A B * C / D *", methodInfo.Invoke(test8, new object[] { }));

            ExpressionTree test9 = new ExpressionTree("(A+B*C)/D+(E*F*G+H)");
            methodInfo = this.GetMethod("ConvertToPostfix", test9, new Type[] { });
            Assert.AreEqual("A B C * + D / E F * G * H + +", methodInfo.Invoke(test9, new object[] { }));

            ExpressionTree test10 = new ExpressionTree("(A*B+C)/D+(E+F*G+H)+(I+J*K)/L+(M*N*O+P)+(Q+R*S)/T+(U*V*W+X)");
            methodInfo = this.GetMethod("ConvertToPostfix", test10, new Type[] { });
            Assert.AreEqual("A B * C + D / E F G * + H + + I J K * + L / + M N * O * P + + Q R S * + T / + U V * W * X + +", methodInfo.Invoke(test10, new object[] { }));
        }

        /// <summary>
        /// test to set variables and evaluate expression.
        /// </summary>
        [Test]
        public void TestEvaluate()
        {
            // Boundary Case
            ExpressionTree test = new ExpressionTree("A1+B1+C1");

            Assert.AreEqual(0, test.Evaluate());

            // Normal Cases
            test.SetVariable("A1", 5.0);
            test.SetVariable("B1", 10.0);
            test.SetVariable("C1", 1.0);

            Assert.AreEqual(16.0, test.Evaluate());

            ExpressionTree test2 = new ExpressionTree("A1-B1-C1-2");

            test2.SetVariable("A1", 10.0);
            test2.SetVariable("B1", 5.0);
            test2.SetVariable("C1", 1.0);

            Assert.AreEqual(2.0, test2.Evaluate());

            ExpressionTree test3 = new ExpressionTree("Hello-12-World");

            test3.SetVariable("Hello", 42.0);
            test3.SetVariable("World", 20.0);

            Assert.AreEqual(10.0, test3.Evaluate());

            ExpressionTree test4 = new ExpressionTree("A1-B1-C1");

            Assert.AreEqual(0.0, test4.Evaluate());

            ExpressionTree test5 = new ExpressionTree("aaaaaa-bbbbbb-cccccc-50");

            test5.SetVariable("aaaaaa", 100.0);
            test5.SetVariable("bbbbbb", 10.0);
            test5.SetVariable("cccccc", 1.0);

            Assert.AreEqual(39.0, test5.Evaluate());

            ExpressionTree test6 = new ExpressionTree("aAaAaA-bBbBbB-cCcCcC-0");

            test6.SetVariable("aAaAaA", 100.0);
            test6.SetVariable("bBbBbB", 10.0);
            test6.SetVariable("cCcCcC", 1.0);

            Assert.AreEqual(89.0, test6.Evaluate());

            ExpressionTree test7 = new ExpressionTree("(A - 2) + (10 - A)");

            test7.SetVariable("A", 5);

            Assert.AreEqual(8.0, test7.Evaluate());

            ExpressionTree test8 = new ExpressionTree("10");

            Assert.AreEqual(10.0, test8.Evaluate());

            // Edge Cases
            ExpressionTree test9 = new ExpressionTree("(A+2)+(6-B)+c");

            test9.SetVariable("A", 2);
            test9.SetVariable("B", 2);
            test9.SetVariable("c", 3);

            Assert.AreEqual(11, test9.Evaluate());

            ExpressionTree test10 = new ExpressionTree("A+B+C1+Hello+6");

            test10.SetVariable("A", 40);
            test10.SetVariable("B", 21);
            test10.SetVariable("C1", 5);
            test10.SetVariable("Hello", 0);

            Assert.AreEqual(72.0, test10.Evaluate());
        }

        //-------------------- HW 6 Tests --------------------------

        /// <summary>
        /// Test method to test the evaluation of parenthesis.
        /// </summary>
        [Test]
        public void TestParenthesis()
        {
            // Normal Case
            ExpressionTree test = new ExpressionTree("(A1 + A2) - A3");
            test.SetVariable("A1", 10);
            test.SetVariable("A2", 10);
            test.SetVariable("A3", 5);
            Assert.AreEqual(15, test.Evaluate());

            ExpressionTree test2 = new ExpressionTree("(A1 + a1A) - (1a3 + Aa1)");
            test2.SetVariable("A1", 25);
            test2.SetVariable("a1A", 20);
            test2.SetVariable("1a3", 10);
            test2.SetVariable("Aa1", 5);
            Assert.AreEqual(30, test2.Evaluate());

            ExpressionTree test3 = new ExpressionTree("(HelloWorld + helloworld) - (5 * h1h1h1h1h1)");
            test3.SetVariable("HelloWorld", 25);
            test3.SetVariable("helloworld", 20);
            test3.SetVariable("h1h1h1h1h1", 5);
            Assert.AreEqual(20, test3.Evaluate());

            // Edge Case
            ExpressionTree test4 = new ExpressionTree("hello - 5");
            test4.SetVariable("hello", -16);
            Assert.AreEqual(-21, test4.Evaluate());

            // Boundary Case
            ExpressionTree test5 = new ExpressionTree("(A1)");
            Assert.AreEqual(0, test5.Evaluate());

            ExpressionTree test6 = new ExpressionTree("(A1)");
            test6.SetVariable("A1", -6);
            Assert.AreEqual(-6, test6.Evaluate());

            ExpressionTree test7 = new ExpressionTree("5 * (2 - 5)");
            Assert.AreEqual(-15, test7.Evaluate());

            ExpressionTree test8 = new ExpressionTree("(5 * 2) * 2 - 5");
            Assert.AreEqual(15, test8.Evaluate());

            ExpressionTree test9 = new ExpressionTree("5-(4*3)+(2-1)");
            Assert.AreEqual(-6, test9.Evaluate());

            ExpressionTree test10 = new ExpressionTree("(10*2)- (0 * 5) +(5 + 3) - 3");
            Assert.AreEqual(25, test10.Evaluate());
        }

        /// <summary>
        /// Test method to test the multiplication and divison operator evaluation.
        /// </summary>
        [Test]
        public void TestMultiplicationDivison()
        {
            ExpressionTree test = new ExpressionTree("a * b * b / 2");
            test.SetVariable("a", 3);
            test.SetVariable("b", 5);

            Assert.AreEqual(37.5, test.Evaluate());

            // edge case
            ExpressionTree test2 = new ExpressionTree("a * b * b / 2");
            test2.SetVariable("a", 3);
            test2.SetVariable("b", 0);

            Assert.AreEqual(0, test2.Evaluate());

            ExpressionTree test3 = new ExpressionTree("0 * 0 / 9999999999999999999999999999999999");
            Assert.AreEqual(0, test3.Evaluate());

            // boundary case
            ExpressionTree test4 = new ExpressionTree("GoCougs123 * 1");
            test4.SetVariable("GoCougs123", 1.7976931348623157E+308);
            Assert.AreEqual(1.7976931348623157E+308, test4.Evaluate());

            ExpressionTree test5 = new ExpressionTree("GoCougs123 / 5 * (30 / 2)");
            test5.SetVariable("GoCougs123", 50);
            Assert.AreEqual(150, test5.Evaluate());

            ExpressionTree test6 = new ExpressionTree("100 / (GoCougs123 / 5) * (30 / 2)");
            test6.SetVariable("GoCougs123", 50);
            Assert.AreEqual(150, test6.Evaluate());

            ExpressionTree test7 = new ExpressionTree("100.2 * 50.5 / 2");
            Assert.AreEqual(2530.05, test7.Evaluate());

            ExpressionTree test8 = new ExpressionTree("(100.2 * 50.5 / 2) * 0");
            Assert.AreEqual(0, test8.Evaluate());

            ExpressionTree test9 = new ExpressionTree("100 / 50 + 2 * 0");
            Assert.AreEqual(2, test9.Evaluate());

            ExpressionTree test10 = new ExpressionTree("360 / 6 * 2 / 2 + 99999 * 0");
            Assert.AreEqual(60, test10.Evaluate());
        }

        // ---------------------------- HW 7 Tests -------------------------------

        /// <summary>
        /// Test method to test the evaluation of the cell.
        /// </summary>
        [Test]
        public void TestCellEvaluation()
        {
            Spreadsheet test = new Spreadsheet(15, 15);

            // normal case
            // Set A1 to 50 calculate A1+5 at B2;
            test.GetCell(0, 0).Text = "50";
            test.GetCell(1, 1).Text = "=A1+5";
            Assert.AreEqual("55", test.GetCell(1, 1).Value);

            test.GetCell(3, 3).Text = "=B2*2";
            Assert.AreEqual("110", test.GetCell(3, 3).Value);

            test.GetCell(5, 5).Text = "=D4";
            Assert.AreEqual("110", test.GetCell(5, 5).Value);

            // Edge Case
            test.GetCell(2, 1).Text = "Hello World";
            Assert.AreEqual("Hello World", test.GetCell(2, 1).Value);
            test.GetCell(2, 4).Text = "=B3";
            Assert.AreEqual("Hello World", test.GetCell(2, 4).Value);

            test.GetCell(6, 6).Text = "=(5 * 2) * 2 - 5";
            Assert.AreEqual("15", test.GetCell(6, 6).Value);

            test.GetCell(6, 6).Text = "=3/2";
            Assert.AreEqual("1.5", test.GetCell(6, 6).Value);

            test.GetCell(6, 6).Text = "=1 - 2";
            Assert.AreEqual("-1", test.GetCell(6, 6).Value);

            test.GetCell(10, 10).Text = "=5000";
            Assert.AreEqual("5000", test.GetCell(10, 10).Value);

            test.GetCell(12, 12).Text = "=10/0";
            Assert.AreEqual("#DIV/0!", test.GetCell(12, 12).Value);

            test.GetCell(8, 8).Text = "=M13+2";
            Assert.AreEqual("#DIV/0!", test.GetCell(8, 8).Value);

            // Boundary Case
            test.GetCell(5, 2).Text = "=";
            Assert.AreEqual("=", test.GetCell(5, 2).Value);

            test.GetCell(6, 1).Text = "=5234A";
            Assert.AreEqual("#NAME?", test.GetCell(6, 1).Value);

            test.GetCell(6, 1).Text = "=B3+2";
            Assert.AreEqual("#VALUE!", test.GetCell(6, 1).Value);

            test.GetCell(8, 1).Text = "=hello+2";
            Assert.AreEqual("#NAME?", test.GetCell(8, 1).Value);

            test.GetCell(6, 1).Text = "=D3";
            Assert.AreEqual("0", test.GetCell(6, 1).Value);
        }

        /// <summary>
        /// Test method to check that the cell that is referencing the changed cell is updating.
        /// </summary>
        [Test]
        public void TestCellReferenceUpdate()
        {
            Spreadsheet test = new Spreadsheet(4, 4);

            test.GetCell(0, 0).Text = "3";
            test.GetCell(1, 0).Text = "=A1"; // A2 = 3
            test.GetCell(0, 0).Text = "12"; // A2 = 12
            Assert.AreEqual("12", test.GetCell(1, 0).Value);

            test.GetCell(2, 0).Text = "=A2+5"; // A3 = 12 + 5
            test.GetCell(0, 0).Text = "6"; // A3 = 6 + 5
            Assert.AreEqual("6", test.GetCell(1, 0).Value); // A2 = 6
            Assert.AreEqual("11", test.GetCell(2, 0).Value); // A3 = 11

            test.GetCell(3, 0).Text = "=A3 + 6"; // A4 = 11 + 6
            Assert.AreEqual("17", test.GetCell(3, 0).Value);
            test.GetCell(2, 0).Text = "=A2 + 10"; // A3 = 16
            Assert.AreEqual("22", test.GetCell(3, 0).Value); // A4 = 22

            test.GetCell(0, 0).Text = "1"; // A1 = 1
            Assert.AreEqual("1", test.GetCell(1, 0).Value); // A2 = 1
            Assert.AreEqual("11", test.GetCell(2, 0).Value); // A3 = 11
            Assert.AreEqual("17", test.GetCell(3, 0).Value); // A4 = 17
        }

        // ---------------------------- HW 8 Tests ---------------------------------

        /// <summary>
        /// Test method to test the bgcolor property of the cell.
        /// </summary>
        [Test]
        public void TestCellBackgroundColorChange()
        {
            Spreadsheet test = new Spreadsheet(2, 2);

            Assert.AreEqual(0xFFFFFFFF, test.GetCell(0, 0).BGColor);

            test.GetCell(0, 0).BGColor = 0x12345678;

            Assert.AreEqual(0x12345678, test.GetCell(0, 0).BGColor);

            Assert.AreEqual(0xFFFFFFFF, test.GetCell(1, 1).BGColor);
        }

        /// <summary>
        /// Test method to test the undo and redo functionality of the text.
        /// </summary>
        [Test]
        public void TestTextUndoRedo()
        {
            Spreadsheet test = new Spreadsheet(2, 2);

            test.GetCell(0, 0).Text = "=1+1";
            test.GetCell(0, 0).Text = "=2+2";
            test.Invoker.AddUndo(new TextCommand(test.GetCell(0, 0), "=2+2", "=1+1"), "cell text change");
            test.Invoker.Undo();
            Assert.AreEqual("2", test.GetCell(0, 0).Value);
            test.Invoker.Redo();
            Assert.AreEqual("4", test.GetCell(0, 0).Value);

            test.GetCell(0, 0).Text = "a";
            test.GetCell(0, 0).Text = "b";
            test.Invoker.AddUndo(new TextCommand(test.GetCell(0, 0), "b", "a"), "cell text change");
            test.GetCell(0, 0).Text = "c";
            test.Invoker.AddUndo(new TextCommand(test.GetCell(0, 0), "c", "b"), "cell text change");
            test.GetCell(0, 0).Text = "d";
            test.Invoker.AddUndo(new TextCommand(test.GetCell(0, 0), "d", "c"), "cell text change");
            test.Invoker.Undo();
            test.Invoker.Undo();
            test.Invoker.Undo();
            Assert.AreEqual("a", test.GetCell(0, 0).Value);
            test.Invoker.Redo();
            Assert.AreEqual("b", test.GetCell(0, 0).Value);
            test.Invoker.Redo();
            Assert.AreEqual("c", test.GetCell(0, 0).Value);
            test.Invoker.Redo();
            Assert.AreEqual("d", test.GetCell(0, 0).Value);
        }

        /// <summary>
        /// test method to test the color undo and redo.
        /// </summary>
        [Test]
        public void TestColorUndoRedo()
        {
            Spreadsheet test = new Spreadsheet(2, 2);
            List<Cell> selected = new List<Cell>();
            List<uint> oldColors = new List<uint>();

            selected.Add(test.GetCell(1, 1));
            oldColors.Add(test.GetCell(1, 1).BGColor);
            test.Invoker.AddUndo(new ColorCommand(1U, selected, oldColors), "changing cell background color");

            test.GetCell(1, 1).BGColor = 1U;
            test.Invoker.Undo();
            Assert.AreEqual(0xFFFFFFFF, test.GetCell(1, 1).BGColor);
            test.Invoker.Redo();
            Assert.AreEqual(1U, test.GetCell(1, 1).BGColor);

            oldColors.Clear();
            oldColors.Add(test.GetCell(1, 1).BGColor);
            test.Invoker.AddUndo(new ColorCommand(5U, selected, oldColors), "changing cell background color");
            test.GetCell(1, 1).BGColor = 5U;

            Assert.AreEqual(5U, test.GetCell(1, 1).BGColor);

            oldColors.Clear();
            oldColors.Add(test.GetCell(1, 1).BGColor);
            test.Invoker.AddUndo(new ColorCommand(3U, selected, oldColors), "changing cell background color");
            test.GetCell(1, 1).BGColor = 3U;

            Assert.AreEqual(3U, test.GetCell(1, 1).BGColor);

            oldColors.Clear();
            oldColors.Add(test.GetCell(1, 1).BGColor);
            test.Invoker.AddUndo(new ColorCommand(4U, selected, oldColors), "changing cell background color");
            test.GetCell(1, 1).BGColor = 4U;

            Assert.AreEqual(4U, test.GetCell(1, 1).BGColor);
            test.Invoker.Undo();

            Assert.AreEqual(3U, test.GetCell(1, 1).BGColor);
            test.Invoker.Redo();

            Assert.AreEqual(4U, test.GetCell(1, 1).BGColor);
        }

        // ---------------------------- HW 9 Tests ---------------------------------

        /// <summary>
        /// test method to test the functionality of saving an xml file.
        /// </summary>
        [Test]
        public void SaveTest()
        {
            // get the current directory of the application
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // set the value of the cells
            Spreadsheet test = new Spreadsheet(5, 5);
            test.GetCell(0, 0).Text = "HELLO WORLD!";
            test.GetCell(0, 0).BGColor = 9U;

            test.GetCell(1, 1).Text = "MY NAME IS HIEN!";
            test.GetCell(1, 1).BGColor = 100U;

            test.GetCell(2, 2).Text = "=A2+B1";

            test.GetCell(1, 0).BGColor = 8U;

            // create the file to be saved
            Stream savestream = File.Open(path + "/../../../savetest.xml", FileMode.Create);

            // save and close the file stream
            test.Save(savestream);
            savestream.Dispose();
            savestream.Close();

            // open the created file for reading
            XDocument reader = XDocument.Load(path + "/../../../savetest.xml");

            // perform the test by reading the xml file and checking if the values we saved is there.
            foreach (XElement tag in reader.Root.Elements("cell"))
            {
                switch (tag.Attribute("name").Value)
                {
                    case "A1":
                        Assert.AreEqual("HELLO WORLD!", tag.Element("text").Value);
                        Assert.AreEqual(9U, uint.Parse(tag.Element("bgcolor").Value));
                        break;

                    case "B2":
                        Assert.AreEqual("MY NAME IS HIEN!", tag.Element("text").Value);
                        Assert.AreEqual(100U, uint.Parse(tag.Element("bgcolor").Value));
                        break;
                    case "C3":
                        Assert.AreEqual("=A2+B1", tag.Element("text").Value);
                        Assert.AreEqual(0xFFFFFFFF, uint.Parse(tag.Element("bgcolor").Value));
                        break;
                    case "A2":
                        Assert.AreEqual(string.Empty, tag.Element("text").Value);
                        Assert.AreEqual(8U, uint.Parse(tag.Element("bgcolor").Value));
                        break;
                }
            }

            // delete the file when done testing
            File.Delete(path + "/../../../savetest.xml");
        }

        /// <summary>
        /// method to test the functionality of loading the xml file.
        /// </summary>
        [Test]
        public void LoadTest()
        {
            // get the current directory of the application
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Spreadsheet test = new Spreadsheet(5, 5);

            // create the file to be loaded
            Stream savestream = File.Open(path + "/../../../loadtest.xml", FileMode.Create);

            XmlWriter writer = XmlWriter.Create(savestream);

            writer.WriteStartElement("spreadsheet");

            // CELL 1
            writer.WriteStartElement("cell");
            writer.WriteAttributeString("some_random_attribute", "some_random_value");
            writer.WriteAttributeString("name", "A1");
            writer.WriteAttributeString("some_random_attribute2", "some_random_value2");

            writer.WriteStartElement("SOME_RANDOM_TAG");
            writer.WriteString("helooooooooooooo");
            writer.WriteEndElement();

            writer.WriteStartElement("text");
            writer.WriteString("HELLO WORLD!");
            writer.WriteEndElement();

            writer.WriteStartElement("SOME_RANDOM_TAG2");
            writer.WriteString("HEYYYYYYYYYYYYYYYYYY");
            writer.WriteEndElement();

            writer.WriteStartElement("bgcolor");
            writer.WriteString(9U.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            // CELL 2
            writer.WriteStartElement("cell");
            writer.WriteAttributeString("name", "B2");

            writer.WriteStartElement("text");
            writer.WriteString("=A1");
            writer.WriteEndElement();

            writer.WriteStartElement("bgcolor");
            writer.WriteString(100U.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            // CELL 3
            writer.WriteStartElement("cell");
            writer.WriteAttributeString("name", "C3");

            writer.WriteStartElement("bgcolor");
            writer.WriteString(123U.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("text");
            writer.WriteString("=5+10");
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();

            // close the writer
            writer.Dispose();
            writer.Close();

            // close the writing stream
            savestream.Dispose();
            savestream.Close();

            // load the file.
            Stream loadstream = File.Open(path + "/../../../loadtest.xml", FileMode.Open);
            test.Load(loadstream);

            // perform the test
            Assert.AreEqual("HELLO WORLD!", test.GetCell(0, 0).Value);
            Assert.AreEqual(9U, test.GetCell(0, 0).BGColor);

            Assert.AreEqual("HELLO WORLD!", test.GetCell(1, 1).Value);
            Assert.AreEqual("=A1", test.GetCell(1, 1).Text);
            Assert.AreEqual(100U, test.GetCell(1, 1).BGColor);

            Assert.AreEqual("15", test.GetCell(2, 2).Value);
            Assert.AreEqual("=5+10", test.GetCell(2, 2).Text);
            Assert.AreEqual(123U, test.GetCell(2, 2).BGColor);

            // close dispose and delete the created file
            loadstream.Dispose();
            loadstream.Close();
            File.Delete(path + "/../../../loadtest.xml");
        }

        /// <summary>
        /// method to test the functionality saving and loading the cells.
        /// </summary>
        [Test]
        public void SaveLoadTest()
        {
            // get current directory of the application
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Spreadsheet test = new Spreadsheet(5, 5);
            test.GetCell(0, 0).Text = "Hello World";
            test.GetCell(0, 0).BGColor = 1U;
            test.GetCell(1, 0).Text = "5";
            test.GetCell(1, 1).Text = "=A2+A2";
            test.GetCell(2, 2).BGColor = 2U;
            test.GetCell(4, 4).Text = "=hi+";
            test.GetCell(4, 1).Text = "=A1";

            // create the file to be saved
            Stream savestream = File.Open(path + "/../../../test.xml", FileMode.Create);

            // save and close the file stream
            test.Save(savestream);
            savestream.Dispose();
            savestream.Close();

            // open and load the file
            Stream loadstream = File.Open(path + "/../../../test.xml", FileMode.Open);
            test.Load(loadstream);

            // perform the test
            Assert.AreEqual("Hello World", test.GetCell(0, 0).Value);
            Assert.AreEqual(1U, test.GetCell(0, 0).BGColor);
            Assert.AreEqual("5", test.GetCell(1, 0).Value);
            Assert.AreEqual("=A2+A2", test.GetCell(1, 1).Text);
            Assert.AreEqual("10", test.GetCell(1, 1).Value);
            Assert.AreEqual(2U, test.GetCell(2, 2).BGColor);
            Assert.AreEqual("#ERROR!", test.GetCell(4, 4).Value);
            Assert.AreEqual("=hi+", test.GetCell(4, 4).Text);
            Assert.AreEqual("=A1", test.GetCell(4, 1).Text);
            Assert.AreEqual("Hello World", test.GetCell(4, 1).Value);

            // Deleting the created file after the test is done
            loadstream.Dispose();
            loadstream.Close();
            File.Delete(path + "/../../../test.xml");
        }

        // ---------------------------- HW 10 Tests ---------------------------------
        // My spreadsheet already handles bad references and other errors
        // So, for HW10 all I need to implement is handling self references and circular references

        /// <summary>
        /// Method to test self reference.
        /// </summary>
        [Test]
        public void SelfReferenceTest()
        {
            Spreadsheet test = new Spreadsheet(5, 5);
            test.GetCell(0, 0).Text = "=A1";
            test.GetCell(1, 1).Text = "5";
            test.GetCell(1, 1).Text = "=B2";
            test.GetCell(2, 2).Text = "=A1";
            test.GetCell(3, 3).Text = "=D4";

            Assert.AreEqual("#SELFREF!", test.GetCell(0, 0).Value);
            Assert.AreEqual("#SELFREF!", test.GetCell(1, 1).Value);
            Assert.AreEqual("#SELFREF!", test.GetCell(2, 2).Value);
            Assert.AreEqual("#SELFREF!", test.GetCell(3, 3).Value);

            test.GetCell(0, 0).Text = "10";

            Assert.AreEqual("10", test.GetCell(0, 0).Value);
        }

        /// <summary>
        /// Method to test circular reference.
        /// </summary>
        [Test]
        public void CircularReferenceTest()
        {
            Spreadsheet test = new Spreadsheet(5, 5);
            test.GetCell(0, 0).Text = "=A2";
            test.GetCell(1, 0).Text = "=A3";
            test.GetCell(2, 0).Text = "=A1";

            Assert.AreEqual("#CIRCULARREF!", test.GetCell(0, 0).Value);
            Assert.AreEqual("#CIRCULARREF!", test.GetCell(1, 0).Value);
            Assert.AreEqual("#CIRCULARREF!", test.GetCell(2, 0).Value);

            test.GetCell(2, 0).Text = "5";

            Assert.AreEqual("5", test.GetCell(0, 0).Value);
            Assert.AreEqual("5", test.GetCell(1, 0).Value);
            Assert.AreEqual("5", test.GetCell(2, 0).Value);

            test.GetCell(0, 1).Text = "=B2";
            test.GetCell(1, 1).Text = "=5+B3";
            test.GetCell(2, 1).Text = "=B1";

            Assert.AreEqual("#CIRCULARREF!", test.GetCell(0, 1).Value);
            Assert.AreEqual("#CIRCULARREF!", test.GetCell(1, 1).Value);
            Assert.AreEqual("#CIRCULARREF!", test.GetCell(2, 1).Value);
        }

        // -------------------------------------------------------------------------

        /// <summary>
        /// method to get private methods, NOT A TEST.
        /// </summary>
        /// <param name="methodName">the name of the method.</param>
        /// <param name="objectUnderTest">the object that contains the method.</param>
        /// <param name="parameterTypes">the parameter types of the method.</param>
        /// <returns>the private method we want.</returns>
        private MethodInfo GetMethod(string methodName, object objectUnderTest, Type[] parameterTypes)
        {
            // THIS IS NOT A TEST METHOD
            if (string.IsNullOrWhiteSpace(methodName))
            {
                Assert.Fail("methodName cannot be null or whitespace");
            }

            var method = objectUnderTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, parameterTypes, null);
            if (method == null)
            {
                Assert.Fail(string.Format("{0} method not found", methodName));
            }

            return method;
        }
    }
}
