using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Zirpl.CalcEngine.Tests
{
    [TestFixture]
    public class CalculationEngineTests
    {
        [Test]
        public void Array_Test()
        {
            CalculationEngine engine = new CalculationEngine(new ServiceProvider());

            // adjust culture
            var cultureInfo = engine.CultureInfo;
            engine.CultureInfo = CultureInfo.InvariantCulture;

            // test Array variables
            var array = new List<double> {0, 1, 2, 3}.ToArray();
            var strings = new List<string> {"K", "E", "S", "A"}.ToArray();
            engine.Variables.Add("A", array);
            engine.Variables.Add("B", strings);
            engine.Variables.Add("Decimal", 4.5M);
            engine.Variables.Add("Number1", 55);
            engine.Variables.Add("Double", 5.5);
            engine.Variables.Add("D310", 310M);
            engine.Variables.Add("D32", 32M);
            engine.Variables.Add("h", 1D / 60);
            engine.Variables.Add("m", 1D / 60 / 60);
            engine.Test("105m/(4*2380)", 105 * (1D / 60 / 60) / (4 * 2380));
            engine.Test("5h", 5 * 1D / 60);

            engine.Test("D310>=250 && D32<=315", true);

            engine.Test("A", array);
            engine.Test("B", strings);
            engine.Test("Max(Array(5,6,7))", 7);
            engine.Test("Number1", 55);
            engine.Test("Max(Number1, 5,33)", 55);
            engine.Test("Max(Number1, 5.33)", 55);
            engine.Test("Min(Number1, 5,88, Decimal)", 4.5M, "MIN(55, 5, 88, 4.5)/*{4.5}*/");

            Assert.That(new double[] {0, 1, 2, 3}, Is.EquivalentTo(engine.Evaluate("Range(0, 3)") as IEnumerable));
            Assert.That(new double[] {0, 100, 200}, Is.EquivalentTo(engine.Evaluate("Range(0, 200, 100)") as IEnumerable));
            Assert.That(new[] {"K", "E", "S"}, Is.EquivalentTo(engine.Evaluate("Array('K', 'E', 'S')") as IEnumerable));
            Assert.That(new double[] {1, 3}, Is.EquivalentTo(engine.Evaluate("Array(1, 3)") as IEnumerable));

            IList<dynamic> enumerable = engine.Evaluate("Array(1, '3')") as object[];

            Assert.That(new dynamic[] {1, "3"}, Is.EquivalentTo(enumerable));

            Assert.That(new double[] {0, 100, 200, 300, 400}, Is.EquivalentTo(engine.Evaluate("Map(Range(0, 200, 100),Range(100, 400, 100))") as IEnumerable));


            Assert.That(new double[] {0, 1, 2}, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), 3)") as IEnumerable));
            Assert.That(new double[] {0, 1, 2}, Is.EquivalentTo(engine.Evaluate("lt(Range(0, 3), 3)") as IEnumerable));
            Assert.That(new double[] {1}, Is.EquivalentTo(engine.Evaluate("LessThan(Array('1', '4'), 3)") as IEnumerable));
            Assert.That(new double[] {0, 1, 2, 3}, Is.EquivalentTo(engine.Evaluate("LessOrEqual(Range(0, 3), 3)") as IEnumerable));
            Assert.That(new double[] {0, 1, 2, 3}, Is.EquivalentTo(engine.Evaluate("le(Range(0, 3), 3)") as IEnumerable));

            Assert.That(new double[] {2, 3}, Is.EquivalentTo(engine.Evaluate("GreaterThan(Range(0, 3), 1)") as IEnumerable));
            Assert.That(new double[] {2, 3}, Is.EquivalentTo(engine.Evaluate("gt(Range(0, 3), 1)") as IEnumerable));
            Assert.That(new double[] {1, 2, 3}, Is.EquivalentTo(engine.Evaluate("GreaterOrEqual(Range(0, 3), 1)") as IEnumerable));
            Assert.That(new double[] {1, 2, 3}, Is.EquivalentTo(engine.Evaluate("ge(Range(0, 3), 1)") as IEnumerable));
            Assert.That(new double[] {4.5, 5.5}, Is.EquivalentTo(engine.Evaluate("ge(Array(Decimal, Double), 1)") as IEnumerable));
            Assert.That(new String[] {"6", "7"}, Is.EquivalentTo(engine.Evaluate("ArrayString('6;7')") as IEnumerable));

            engine.Test("Contains(Array(1,2), 2)", true);
            engine.Test("Contains(Array(1,2), 3)", false);
            engine.Test("Contains(Array('1','2'), 2)", true);
            engine.Test("Contains('1;2', '2')", true);
            engine.Test("Contains('1;2', 2)", true);
            engine.Test("Contains('2', 2)", true);
            engine.Test("Contains(2, '2')", true);
            engine.Test("Contains(Array(Number1, Double, '5.4'), 5.5)", true);
            engine.Test("Contains(Array(1,2,5,7), Array(5,7))", true);
            engine.Test("Contains(Array(1,2,5,7), Array(3,6))", false);
            engine.Test("Contains('n;m; s;!k', Array('s','u'))", true);
            engine.Test("Contains('n|m|s|!k', Array('s','u'), '|')", true);
            engine.Test("Contains('n;m;s;!k', Array('s','u','k'))", false);
            engine.Test("Contains('n;m;s;!k', Array('s','u','k'))", false);

            engine.Options.Functions.ContainsTrimStartChars = new List<char>() {'0'};
            engine.Test("Contains('04;09;7;92;!08', '0092')", true);
            engine.Options.Functions.ContainsTrimStartChars = new List<char>();
            engine.Test("Contains('04;09;7;92;!08', '0092')", false);
            engine.Test("Contains('04;09;7;0092;!08', '92')", false);

            engine.Test("XLOOKUP('06', Array('06', '05', '04'), Array('25', '26', '27'), '20')", "25");
            engine.Test("XLOOKUP('04', Array('06', '05', '04'), Array('25', '26', '27'), '20')", "27");
            engine.Test("XLOOKUP('04', Array('06', '05', '04'), Array(25, 26, 27), '20')", 27);
            engine.Test("XLOOKUP('07', Array('06', '05', '04'), Array('25', '26', '27'), '20')", "20");

            engine.Variables.Add("dec1", 4.5M);
            engine.Variables.Add("doub1", 4.5);
            engine.Test("XLOOKUP(dec1, Array(doub1, 5, 6), Array(7, 8, 9), 0)", 7);

            engine.Options.Functions.ContainsTrimStartChars = new List<char>() {'0'};
            engine.Test("1.2+IF((Contains('0018;0004;0014;0049', '18') && Contains('0092', '92')), 1, 0)+IF((Contains('0008', '0008') && Contains('0105', '105')), 1.5, 0)", 3.7);

            engine.Test("XLOOKUP('07', Array('06', '05', '04'), Array('25', '26', \r\n'27'), '20'\n)", "20");
            engine.Test("XLOOKUP('07', Array('06', '05', '04'), Array('25' /* 0068 */, '26', '27'), '20')", "20");

            engine.Test("XLOOKUP(7, Array(Range(4, 5), Range(6, 8), Range(9, 10)), Array('25', '26', '27'), '20')", "26");
            engine.Test("XLOOKUP(7, Array(Array(4, 5, 7), Array(6, 8), Array(9, 10)), Array('25', '26', '27'), '20')", "25");
        }

        [Test]
        public static void DataContext_Tests()
        {
            CalculationEngine engine = new CalculationEngine(new ServiceProvider());

            // adjust culture
            var cultureInfo = engine.CultureInfo;
            engine.CultureInfo = CultureInfo.InvariantCulture;

            var p = TestPerson.CreateTestPerson();
            p.Parent = TestPerson.CreateTestPerson();
            engine.DataContext = p;

            engine.Variables.Add("sp", 1);

            var s = @"3600 / (1sp * XLOOKUP( CONCATENATE(Specs('jaty'), '-', IF( Number(Specs('jawd')) >= 0068 && Number(Specs('jawd')) < 0110, '68/110', IF(Number(Specs('jawd')) >= 0110 && Number(Specs('jawd')) < 0150, '110/150', Specs('jawd') ) ), '-', IF(Contains('JH', Specs('jaap')), 'JH', 'JL'), '-', IF(Contains('PK;PH', Specs('jspe')), 'OV', 'VOL')), Array( '04-68/110-JL-VOL', '04-68/110-JL-OV', '04-68/110-JH-VOL', '04-68/110-JH-OV', '14-68/110-JL-VOL', '14-68/110-JL-OV', '14-68/110-JH-VOL', '14-68/110-JH-OV', '29-68/110-JL-VOL', '29-68/110-JL-OV', '29-68/110-JH-VOL', '29-68/110-JH-OV', '49-68/110-JL-VOL', '49-68/110-JL-OV', '49-68/110-JH-VOL', '49-68/110-JH-OV', '28-68/110-JL-VOL', '28-68/110-JL-OV', '28-68/110-JH-VOL', '28-68/110-JH-OV', '38-68/110-JL-VOL', '38-68/110-JL-OV', '38-68/110-JH-VOL', '38-68/110-JH-OV', '05-68/110-JL-VOL', '05-68/110-JL-OV', '05-68/110-JH-VOL', '05-68/110-JH-OV', '06-68/110-JL-VOL', '06-68/110-JL-OV', '06-68/110-JH-VOL', '06-68/110-JH-OV', '06-110/150-JL-VOL', '06-110/150-JL-OV', '06-110/150-JH-VOL', '06-110/150-JH-OV', '32-68/110-JL-VOL', '32-68/110-JL-OV', '32-68/110-JH-VOL', '32-68/110-JH-OV', '36-68/110-JL-VOL', '36-68/110-JL-OV', '36-68/110-JH-VOL', '36-68/110-JH-OV', '36-110/150-JL-VOL', '36-110/150-JL-OV', '36-110/150-JH-VOL', '36-110/150-JH-OV', '86-68/110-JL-VOL', '86-68/110-JL-OV', '86-68/110-JH-VOL', '86-68/110-JH-OV', 'C7-68/110-JL-VOL', 'C7-68/110-JL-OV', 'C7-68/110-JH-VOL', 'C7-68/110-JH-OV', 'C7-110/150-JL-VOL', 'C7-110/150-JL-OV', 'C7-110/150-JH-VOL', 'C7-110/150-JH-OV', '26-110/150-JL-VOL', '26-110/150-JL-OV', '26-110/150-JH-VOL', '26-110/150-JH-OV', '46-110/150-JL-VOL', '46-110/150-JL-OV', '46-110/150-JH-VOL', '46-110/150-JH-OV', '55-68/110-JL-VOL', '55-68/110-JL-OV', '55-68/110-JH-VOL', '55-68/110-JH-OV' ), Array( 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 37, 37, 41, 38, 37, 37, 41, 38, 39, 42, 35, 22 ), 38))";

            p.Specs.Add("jaty", "04");
            p.Specs.Add("jawd", "0092");
            p.Specs.Add("jspe", "00");
            p.Specs.Add("jaap", "JH");

            engine.Test(s, 76.5957446808511M);
            Assert.AreEqual("(3600 / 1 * 1 * XLOOKUP(CONCATENATE('04', '-', IF('68/110', IF('110/150', Specs('jawd'))/*{}*/)/*{'68/110'}*/, '-', IF(CONTAINS('JH', 'JH')/*{True}*/, 'JH', 'JL')/*{'JH'}*/, '-', IF(CONTAINS('PK;PH', '00')/*{False}*/, 'OV', 'VOL')/*{'VOL'}*/)/*{'04-68/110-JH-VOL'}*/, ['04-68/110-JL-VOL', '04-68/110-JL-OV', '04-68/110-JH-VOL', '04-68/110-JH-OV', '14-68/110-JL-VOL', '14-68/110-JL-OV', '14-68/110-JH-VOL', '14-68/110-JH-OV', '29-68/110-JL-VOL', '29-68/110-JL-OV', '29-68/110-JH-VOL', '29-68/110-JH-OV', '49-68/110-JL-VOL', '49-68/110-JL-OV', '49-68/110-JH-VOL', '49-68/110-JH-OV', '28-68/110-JL-VOL', '28-68/110-JL-OV', '28-68/110-JH-VOL', '28-68/110-JH-OV', '38-68/110-JL-VOL', '38-68/110-JL-OV', '38-68/110-JH-VOL', '38-68/110-JH-OV', '05-68/110-JL-VOL', '05-68/110-JL-OV', '05-68/110-JH-VOL', '05-68/110-JH-OV', '06-68/110-JL-VOL', '06-68/110-JL-OV', '06-68/110-JH-VOL', '06-68/110-JH-OV', '06-110/150-JL-VOL', '06-110/150-JL-OV', '06-110/150-JH-VOL', '06-110/150-JH-OV', '32-68/110-JL-VOL', '32-68/110-JL-OV', '32-68/110-JH-VOL', '32-68/110-JH-OV', '36-68/110-JL-VOL', '36-68/110-JL-OV', '36-68/110-JH-VOL', '36-68/110-JH-OV', '36-110/150-JL-VOL', '36-110/150-JL-OV', '36-110/150-JH-VOL', '36-110/150-JH-OV', '86-68/110-JL-VOL', '86-68/110-JL-OV', '86-68/110-JH-VOL', '86-68/110-JH-OV', 'C7-68/110-JL-VOL', 'C7-68/110-JL-OV', 'C7-68/110-JH-VOL', 'C7-68/110-JH-OV', 'C7-110/150-JL-VOL', 'C7-110/150-JL-OV', 'C7-110/150-JH-VOL', 'C7-110/150-JH-OV', '26-110/150-JL-VOL', '26-110/150-JL-OV', '26-110/150-JH-VOL', '26-110/150-JH-OV', '46-110/150-JL-VOL', '46-110/150-JL-OV', '46-110/150-JH-VOL', '46-110/150-JH-OV', '55-68/110-JL-VOL', '55-68/110-JL-OV', '55-68/110-JH-VOL', '55-68/110-JH-OV'], [38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 38, 38, 47, 47, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 39, 42, 35, 38, 39, 42, 35, 38, 37, 37, 41, 38, 37, 37, 41, 38, 37, 37, 41, 38, 39, 42, 35, 22], 38)/*{47}*/)/*{76.59574468085107}*/", engine.ParsedExpression);

            engine.Test("Name", "Test Person");
            engine.Test("1h", 1D / 60);

            engine.Test("1mp*ChildrenDct('Test Child 2').Age", p.ChildrenDct["Test Child 2"].Age.Value);

            engine.Variables.Add("am", 1);
            engine.Test("HASH(Name)", "53A4E9EC08910DE9BB6EDAA99F8C867C");
            engine.Test("HASH('Name')", "49EE3087348E8D44E1FEDA1917443987");
            engine.Test("CONCATENATE(am, HASH('Name'))", "149EE3087348E8D44E1FEDA1917443987");
            engine.Functions.Remove("CODE");
            Assert.IsTrue(engine.Validate<bool>("Code='Code'"));
            engine.Test("Parent.Name", "Test Person");
            engine.Test("Name.Length * 2", p.Name.Length * 2);
            engine.Test("Children.Count", p.Children.Count);
            engine.Test("Children(2).Name", p.Children[2].Name);

            engine.Test("15*ChildrenDct(\"Test Child 2\").Age+14", (15 * p.ChildrenDct["Test Child 2"].Age + 14).Value);
            engine.Test("ChildrenDct('Test Child 2').Name", p.ChildrenDct["Test Child 2"].Name);
            engine.Test("ValueOr(ChildrenDct('Test Child 2').Nullable, 0) <> 0", false);
            engine.Test("ValueOr(ChildrenDct('Test Child 2').Nullable, 0) < 6", true);
            engine.Test("ValueOr(ChildrenDct('Test Child 2').Number, 0) <> 0", true);
            engine.ThrowOnInvalidBindingExpression = false;
            engine.Test("ValueOr(ChildrenDct('NotExist').Number, 3) == 3", true);
            engine.ThrowOnInvalidBindingExpression = true;
            Assert.Throws<CalcEngineBindingException>(() => { engine.Test("ChildrenDct('NotExist').Number", true); });

            var ex = Assert.Throws<CalcEngineBindingException>(() =>
            {
                var d = engine.Evaluate<double>("ChildrenAgeDct('Test Child 10') * 2");
            });
            Assert.That(ex.Message, Is.EqualTo("'ChildrenAgeDct' of 'Zirpl.CalcEngine.Tests.TestPerson' (TestPerson) don't have key(s) 'Test Child 10'"));

            Assert.AreEqual(0, engine.Evaluate<double>("ChildrenAgeDct('Test Child 10') * 2", false));
            Assert.AreEqual(2, engine.Evaluate<double>("ChildrenAgeDct('Test Child 10') + 2", false));

            engine.Test("ChildrenIdDct('16C5888C-6C75-43DD-A372-2A3398DAE038').Name", p.ChildrenDct["Test Child 1"].Name);
            engine.Test("ChildrenDct.Count", p.ChildrenDct.Count);

            // DataContext functions
            engine.RegisterFunction("GetParent", 0, (calculationEngine, parms) =>
            {
                var testPerson = calculationEngine.DataContext as TestPerson;
                return testPerson.Name;
            });
            engine.Test("GetParent()", p.Name);

            engine.InValidation = true;
            Assert.That(new double[] { }, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenAgeDct('Test Child 2'))") as IEnumerable));
            Assert.That(new double[] { }, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenWeightDct('Test Child 2'))") as IEnumerable));
            Assert.That(new double[] { }, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenSalaryDct('Test Child 2'))") as IEnumerable));

            engine.InValidation = false;
            Assert.That(new double[] {0, 1}, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenAgeDct('Test Child 2'))") as IEnumerable));
            Assert.That(new double[] {0, 1, 2, 3}, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenWeightDct('Test Child 2'))") as IEnumerable));
            Assert.That(new double[] {0, 1, 2, 3}, Is.EquivalentTo(engine.Evaluate("LessThan(Range(0, 3), ChildrenSalaryDct('Test Child 2'))") as IEnumerable));
        }

        [Test]
        public static void GenericDataContext_Tests()
        {
            var testPersonCalculationEngine = new TestPersonCalculationEngine();
            // DataContext functions
            testPersonCalculationEngine.RegisterFunction("GetParent", 0, (calculationEngine, parms) =>
            {
                var personCalculationEngine = calculationEngine as TestPersonCalculationEngine;
                return personCalculationEngine.DataContext.Name;
            });

            var p = TestPerson.CreateTestPerson();
            p.Parent = TestPerson.CreateTestPerson();
            testPersonCalculationEngine.DataContext = p;
            testPersonCalculationEngine.Test("GetParent()", p.Name);
        }

        [Test]
        public void Functions_Test()
        {
            CalculationEngine engine = new CalculationEngine(new ServiceProvider());

            // adjust culture
            var cultureInfo = engine.CultureInfo;
            engine.CultureInfo = CultureInfo.InvariantCulture;

            engine.Test("1492.5373134328358", 1492.5373134328358);
            engine.Test("1 041.341 ", 1041.341);

            // test invalid parsing
            var ex = Assert.Throws<CalcEngineException>(() =>
            {
                var d = engine.Evaluate<string>("Max(1,2,3)KalleKusta");
            });

            Assert.That(ex.Message, Is.EqualTo("Syntax error. Expression: Max(1,2,3)[KalleKusta]"));

            // test internal operators
            engine.Test("0", 0.0);
            engine.Test("+1", 1.0);
            engine.Test("-1", -1.0);

            engine.Test("1+1", 1 + 1.0);
            engine.Test("1*2*3*4*5*6*7*8*9", 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9.0);
            engine.Test("1/(1+1/(1+1/(1+1/(1+1/(1+1/(1+1/(1+1/(1+1/(1+1/(1+1))))))))))", 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1 / (1 + 1.0)))))))))));
            engine.Test("((1+2)*(2+3)/(4+5))^0.123", Math.Pow((1 + 2) * (2 + 3) / (4 + 5.0), 0.123));
            engine.Test("10%", 0.1);
            engine.Test("1e+3", 1000.0);
            engine.Test("'0020'+1", 21);

            // test simple variables
            engine.Variables.Add("one", 1);
            engine.Variables.Add("x", 1);
            engine.Variables.Add("x2", "1");
            engine.Variables.Add("two", 2);
            engine.Test("one + two", 3);
            engine.Test("x2='1'", true);

            engine.Test("2*x+1", 3);
            engine.Test("(two + two)^2", 16);
            engine.Variables.Clear();

            // COMPARE TESTS
            engine.Test("5=5", true);
            engine.Test("'2'='2'", true);
            engine.Test("5==5", true);
            engine.Test("6==5", false);
            engine.Test("6<>5", true);
            engine.Test("6=5", false);
            engine.Test("6<5", false);
            engine.Test("6>5", true);
            engine.Test("5<=10", true);
            engine.Test("5>=3", true);
            engine.Test("'Viis'>='Üks'", false);

            // LOGICAL FUNCTION TESTS

            engine.Test("5<=6.0 && 6>=3", true);
            engine.Test("true", true);
            engine.Test("true  && true", true);
            engine.Test("true  && false", false);
            engine.Test("false && true", false);
            engine.Test("false && false", false);
            engine.Test("true  || true", true);
            engine.Test("true  || false", true);
            engine.Test("false || true", true);
            engine.Test("false || false", false);

            engine.Test("AND(true, true)", true);
            engine.Test("AND(true, false)", false);
            engine.Test("AND(false, true)", false);
            engine.Test("AND(false, false)", false);
            engine.Test("OR(true, true)", true);
            engine.Test("OR(true, false)", true);
            engine.Test("OR(false, true)", true);
            engine.Test("OR(false, false)", false);
            engine.Test("NOT(false)", true);
            engine.Test("NOT(true)", false);
            engine.Test("IF(5 > 4, true, false)", true);
            engine.Test("IF(5 > 14, true, false)", false);
            engine.Test("TRUE()", true);
            engine.Test("FALSE()", false);

            // MATH/TRIG FUNCTION TESTS
            engine.Test("ABS(-12)", 12.0);
            engine.Test("ABS(+12)", 12.0);
            engine.Test("ACOS(.23)", Math.Acos(.23));
            engine.Test("ASIN(.23)", Math.Asin(.23));
            engine.Test("ATAN(.23)", Math.Atan(.23));
            engine.Test("ATAN2(1,2)", Math.Atan2(1, 2));
            engine.Test("CEILING(1.8)", Math.Ceiling(1.8));
            engine.Test("COS(1.23)", Math.Cos(1.23));
            engine.Test("COSH(1.23)", Math.Cosh(1.23));
            engine.Test("EXP(1)", Math.Exp(1));
            engine.Test("FLOOR(1.8)", Math.Floor(1.8));
            engine.Test("INT(1.8)", 1);
            engine.Test("LOG(1.8)", Math.Log(1.8, 10)); // default base is 10
            engine.Test("LOG(1.8, 4)", Math.Log(1.8, 4)); // custom base
            engine.Test("LN(1.8)", Math.Log(1.8)); // real log
            engine.Test("LOG10(1.8)", Math.Log10(1.8)); // same as Log(1.8)
            engine.Test("PI()", Math.PI);
            engine.Test("POWER(2,4)", Math.Pow(2, 4));
            //engine.Test("RAND") <= 1.0);
            //engine.Test("RANDBETWEEN(4,5)") <= 5);
            engine.Test("SIGN(-5)", -1);
            engine.Test("SIGN(+5)", +1);
            engine.Test("SIGN(0)", 0);
            engine.Test("SIN(1.23)", Math.Sin(1.23));
            engine.Test("SINH(1.23)", Math.Sinh(1.23));
            engine.Test("SQRT(144)", Math.Sqrt(144));
            engine.Test("SUM(1, 2, 3, 4)", 1 + 2 + 3 + 4.0);
            engine.Test("MAX(1.4, 2, 3, 4.5)", 4.5);
            engine.Test("MIN(1.4, 2, 3, 4.5)", 1.4);
            engine.Test("TAN(1.23)", Math.Tan(1.23));
            engine.Test("TANH(1.23)", Math.Tanh(1.23));
            engine.Test("TRUNC(1.23)", 1.0);
            engine.Test("PI()", Math.PI);
            engine.Test("PI", Math.PI);
            engine.Test("LN(10)", Math.Log(10));
            engine.Test("LOG(10)", Math.Log10(10));
            engine.Test("EXP(10)", Math.Exp(10));
            engine.Test("SIN(PI()/4)", Math.Sin(Math.PI / 4));
            engine.Test("ASIN(PI()/4)", Math.Asin(Math.PI / 4));
            engine.Test("SINH(PI()/4)", Math.Sinh(Math.PI / 4));
            engine.Test("COS(90)", Math.Cos(90));
            engine.Test("COS(PI()/4)", Math.Cos(Math.PI / 4));
            engine.Test("ACOS(PI()/4)", Math.Acos(Math.PI / 4));
            engine.Test("COSH(PI()/4)", Math.Cosh(Math.PI / 4));
            engine.Test("TAN(PI()/4)", Math.Tan(Math.PI / 4));
            engine.Test("ATAN(PI()/4)", Math.Atan(Math.PI / 4));
            engine.Test("ATAN2(1,2)", Math.Atan2(1, 2));
            engine.Test("TANH(PI()/4)", Math.Tanh(Math.PI / 4));
            engine.Test("Number('0.54')*2", 0.54 * 2);
            engine.Test("Number('0092')", 92);

            // TEXT FUNCTION TESTS
            engine.Test("CHAR(65)", "A");
            //engine.Test("CODE(\"A\")", 65);
            engine.Test("CONCATENATE(\"(a\", \"b)\")", "(ab)");
            engine.Test("CONCATENATE(\"a\", \"b\")", "ab");
            engine.Test("CONCATENATE('a', 'b')", "ab");
            engine.Test("FIND(\"bra\", \"abracadabra\")", 2);
            engine.Test("FIND(\"BRA\", \"abracadabra\")", -1);
            engine.Test("LEFT(\"abracadabra\", 3)", "abr");
            engine.Test("LEFT(\"abracadabra\")", "a");
            engine.Test("LEN(\"abracadabra\")", 11);
            engine.Test("LOWER(\"ABRACADABRA\")", "abracadabra");
            engine.Test("MID(\"abracadabra\", 1, 3)", "abr");
            engine.Test("PROPER(\"abracadabra\")", "Abracadabra");
            engine.Test("REPLACE(\"abracadabra\", 1, 3, \"XYZ\")", "XYZacadabra");
            engine.Test("REPT(\"abr\", 3)", "abrabrabr");
            engine.Test("RIGHT(\"abracadabra\", 3)", "bra");
            engine.Test("SEARCH(\"bra\", \"abracadabra\")", 2);
            engine.Test("SEARCH(\"BRA\", \"abracadabra\")", 2);
            engine.Test("(SEARCH('432', '30X432')>0) || (Search('4DX', '4DX30P')>0)", true);

            engine.Test("SUBSTITUTE(\"abracadabra\", \"a\", \"b\")", "bbrbcbdbbrb");
            engine.Test("T(123)", "123");
            engine.Test("TEXT(1234, \"n2\")", "1,234.00");
            engine.Test("TRIM(\"   hello   \")", "hello");
            engine.Test("TRIM('00hello00', '00', true)", "hello00");
            engine.Test("TRIM('00hello00', '00', false)", "00hello");
            engine.Test("TRIM('00hello00', '00')", "hello");
            engine.Test("UPPER(\"abracadabra\")", "ABRACADABRA");
            engine.Test("VALUE(\"1234\")", 1234.0);
            engine.Test("PadLeft(1254, 6, \"0\")", "001254");
            engine.Test("PadRight(1254, 6, \"0\")", "125400");
            engine.Test("AffixIF(1254, \"p\", \"s\")", "p1254s");

            engine.Test("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\")", "bbcbbcbbc");
            engine.Test("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\", 1)", "bbcabcabc");
            engine.Test("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\", 2)", "abcbbcabc");
            engine.Test("SUBSTITUTE(\"abcabcabc\", \"A\", \"b\", 2)", "abcabcabc"); // case-sensitive!

            // test taken from http://www.codeproject.com/KB/vb/FormulaEngine.aspx
            var a1 = "\"http://j-walk.com/ss/books\"";
            var exp = "RIGHT(A1,LEN(A1)-FIND(CHAR(1),SUBSTITUTE(A1,\"/\",CHAR(1),LEN(A1)-LEN(SUBSTITUTE(A1,\"/\",\"\")))))";
            engine.Test(exp.Replace("A1", a1), "books");


            // STATISTICAL FUNCTION TESTS
            engine.Test("Average(1, 3, 3, 1, true, false, \"hello\")", 2.0);
            engine.Test("AverageA(1, 3, 3, 1, true, false, \"hello\")", (1 + 3 + 3 + 1 + 1 + 0 + 0) / 7.0);
            engine.Test("Count(1, 3, 3, 1, true, false, \"hello\")", 4.0);
            engine.Test("CountA(1, 3, 3, 1, true, false, \"hello\")", 7.0);

            // restore culture
            engine.CultureInfo = cultureInfo;
        }

        [Test]
        public void Parse_Test()
        {
            CalculationEngine engine = new CalculationEngine(new ServiceProvider());

            // test DataContext
            var dc = engine.DataContext;
            var p = TestPerson.CreateTestPerson();
            engine.DataContext = p;

            var expression = engine.Parse("Parent.Children(2).Address");
            var expression1 = engine.Parse("Parent.ChildrenDct('Test Child 58').Address");

            //expression.Validate();
            //expression1.Validate();

            p.Parent = TestPerson.CreateTestPerson();
            p.Parent.Address = new Address {Street = "sdf"};
            Assert.Throws<CalcEngineBindingException>(() => { expression1.Evaluate(); });
        }

        [Test]
        public void Units_Test()
        {
            CalculationEngine engine = new CalculationEngine(new ServiceProvider());

            var p = new UnitModel
            {
                This = TestPerson.CreateTestPerson()
            };

            engine.DataContext = p;

            var expression = engine.Parse("This.Age*Y*5.0000");

            Console.WriteLine(expression.Evaluate());
        }
    }

    public class TestPersonCalculationEngine : CalculationEngine<TestPerson>
    {
        public TestPersonCalculationEngine() : base(new ServiceProvider())
        {
        }
    }

    internal class UnitModel
    {
        public object This { get; set; }

        public double M => 1;
        public double S => M / 60;
        public double H => M * 60;
        public double D => M * 24;
        public double Y => D * 365;
    }
}