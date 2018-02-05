﻿using Gcode.Common.Utils;
using Gcode.Entity;
using Gcode.TestSuite.Infrastructure;
using Gcode.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gcode.TestSuite
{
	/// <summary>
	/// Gcode command tests
	/// </summary>
	[TestClass]
	public class GcodeParserTests
	{
		[TestMethod]
		public void GcodeParserSyntheticTests1()
		{
			foreach (var r in TestSuiteDataSource.TestSyntheticCodes)
			{
				var gcode = GcodeParser.ToGCode(r);
				Assert.IsInstanceOfType(gcode, typeof(GcodeCommandFrame));
			}
		}
		[TestMethod]
		public void GcodeParserTests1()
		{
			var ds = TestSuiteDataSource.Ds100Gcode.Split("\n");
			foreach (var r in ds)
			{
				var gcode = GcodeParser.ToGCode(r);
				Assert.IsInstanceOfType(gcode, typeof(GcodeCommandFrame), $"{r}");
			}
		}
		[TestMethod]
		public void GcodeParserTests2()
		{
			//M206 T3 P200 X89 ; extruder normal steps per mm
			var ds = TestSuiteDataSource.TestSyntheticCodes[0];
			var gcode = GcodeParser.ToGCode(ds);
			Assert.IsNotNull(gcode.M);
			Assert.AreEqual(206, gcode.M.Value);
			Assert.IsNotNull(gcode.T);
			Assert.AreEqual(3, gcode.T.Value);
			Assert.IsNotNull(gcode.P);
			Assert.AreEqual(200, gcode.P.Value);
			Assert.IsNotNull(gcode.X);
			Assert.AreEqual(89, gcode.X.Value);
			Assert.AreEqual("extruder normal steps per mm", gcode.Comment);
		}
		[TestMethod]
		public void GcodeParserTestsCheckSumIcTest1()
		{
			var ds = TestSuiteDataSource.TestSyntheticCodes[0];
			var gcode = GcodeParser.ToGCode(ds);
			gcode.N = 1;
			gcode.CheckSum = GcodeCrc.FrameCrc(gcode);
			var resStr = GcodeParser.ToStringCommand(gcode);

			Assert.IsNotNull(resStr);
			Assert.IsTrue(resStr.Contains($"*{gcode.CheckSum}"));
		}
		[TestMethod]
		public void GcodeOrderSegmentTest1()
		{
			//M206 T3 P200 X89 ;extruder normal steps per mm
			var ds = TestSuiteDataSource.TestSyntheticCodes[0];
			var gcode = GcodeParser.ToGCode(ds);
			gcode.N = 1;
			var dsExpected = $"N{gcode.N} {ds}";
			var res = GcodeParser.ToStringCommand(gcode);
			Assert.AreEqual(dsExpected, res);
		}
		[TestMethod]
		public void GcodeOrderSegmentTest2()
		{
			var ds = TestSuiteDataSource.Ds100Gcode.Split("\n");
			foreach (var d in ds)
			{
				var s = d.Replace("\r", null);
				if (s == ";") continue;
				var gcode = GcodeParser.ToGCode(s);
				var gcodeStr = GcodeParser.ToStringCommand(gcode);
				var expectedResult = $"{s}";
				if (string.IsNullOrWhiteSpace(gcode.Comment))
				{
					Assert.AreEqual(expectedResult.Trim(), gcodeStr.Trim(), gcodeStr.Trim());
				}

			}
		}
		[TestMethod]
		public void GcodeComment()
		{
			const string cmd = ";> ololo G1 X1 Y1 XZ0 ; this frame comment";
			var gcode = GcodeParser.ToGCode(cmd);

			var gcodeRes = GcodeParser.ToStringCommand(gcode);
			Assert.AreEqual(cmd,gcodeRes );
		}
	}
}