using Microsoft.FSharp.Collections;
using NUnit.Framework;
using Oxbow.Gwe.Core.TemplateEngine;
using Oxbow.Gwe.Lang.Ast;

namespace Oxbow.Gwe.UnitTests
{
    [TestFixture]
    public class ExpressionParsingTests
    {
        [Test]
        public void TestBasic()
        {
            Equation ast = GweParser.eval("!Run( var1,!Run2(), \"!Run2(foobar)\", !run3(x , y ))");
            Assert.That(ast.GetType() == typeof (Equation));

            Expr item = ast.Item;
            Assert.That(item.GetType() == typeof (Expr.Func));
            Assert.That(item.IsFunc);

            var rootExpr = item as Expr.Func;
            Assert.That(rootExpr.Item1 == "!Run");
            Assert.That(rootExpr.Item2.GetType() == typeof (FSharpList<Expr>));

            Assert.That(rootExpr.Item2.Length == 4);
            Assert.That(rootExpr.Item2[0].IsText && ((Expr.Text) rootExpr.Item2[0]).Item == "var1");
            Assert.That(rootExpr.Item2[1].IsFunc && ((Expr.Func) rootExpr.Item2[1]).Item1 == "!Run2" && ((Expr.Func) rootExpr.Item2[1]).Item2.Length == 0);
            Assert.That(rootExpr.Item2[2].IsText && ((Expr.Text) rootExpr.Item2[2]).Item == "!Run2(foobar)");
            Assert.That(rootExpr.Item2[3].IsFunc);

            var innerExpr = rootExpr.Item2[3] as Expr.Func;
            Assert.That(innerExpr.Item1 == "!run3");
            Assert.That(innerExpr.Item2.Length == 2);
            Assert.That(innerExpr.Item2[0].IsText && ((Expr.Text) innerExpr.Item2[0]).Item == "x");
            Assert.That(innerExpr.Item2[1].IsText && ((Expr.Text) innerExpr.Item2[1]).Item == "y");

            ExpressionEvaluator.ParseEquation("!xpath(/my:personnelRequisitionForm/my:reportsTo, !run(234))");
            ExpressionEvaluator.ParseEquation("!xpath(/[]+-*&^%#@, !run(234))");
        }
    }
}