using System.Text;
using Xunit.Abstractions;

namespace ems_back.Tests.Utilities
{
	public class TestReportGenerator : IDisposable
	{
		private readonly ITestOutputHelper _output;
		private readonly StringBuilder _html;
		private readonly List<TestResult> _results = new();
		private readonly DateTime _startTime = DateTime.Now;

		public TestReportGenerator(ITestOutputHelper output)
		{
			_output = output;
			_html = new StringBuilder();
			InitializeReport();
		}

		private void InitializeReport()
		{
			_html.AppendLine("<!DOCTYPE html>");
			_html.AppendLine("<html lang='en'>");
			_html.AppendLine("<head>");
			_html.AppendLine("<meta charset='UTF-8'>");
			_html.AppendLine("<title>EMS Backend Test Report</title>");
			_html.AppendLine("<style>");
			_html.AppendLine("body { font-family: Arial, sans-serif; margin: 2rem; }");
			_html.AppendLine(".test { padding: 1rem; margin-bottom: 1rem; border-radius: 5px; }");
			_html.AppendLine(".passed { background-color: #e8f5e9; border-left: 5px solid #4caf50; }");
			_html.AppendLine(".failed { background-color: #ffebee; border-left: 5px solid #f44336; }");
			_html.AppendLine(".summary { background-color: #f5f5f5; padding: 1rem; border-radius: 5px; }");
			_html.AppendLine("</style>");
			_html.AppendLine("</head>");
			_html.AppendLine("<body>");
			_html.AppendLine("<h1>EMS Backend Test Report</h1>");
			_html.AppendLine($"<p>Generated: {_startTime:yyyy-MM-dd HH:mm:ss}</p>");
			_html.AppendLine("<div class='summary'>");
			_html.AppendLine("<h2>Summary</h2>");
			_html.AppendLine("<div id='summary-stats'></div>");
			_html.AppendLine("</div>");
			_html.AppendLine("<h2>Test Results</h2>");
			_html.AppendLine("<div id='test-results'>");
		}

		public void AddTestResult(string testName, bool passed, TimeSpan duration, string message = null)
		{
			var result = new TestResult
			{
				Name = testName,
				Passed = passed,
				Duration = duration,
				Message = message
			};
			_results.Add(result);

			_html.AppendLine($"<div class='test {(passed ? "passed" : "failed")}'>");
			_html.AppendLine($"<h3>{testName}</h3>");
			_html.AppendLine($"<p>Status: <strong>{(passed ? "PASSED" : "FAILED")}</strong></p>");
			_html.AppendLine($"<p>Duration: {duration.TotalMilliseconds}ms</p>");
			if (!string.IsNullOrEmpty(message))
			{
				_html.AppendLine($"<p>Details: {message}</p>");
			}
			_html.AppendLine("</div>");
		}

		public void Dispose()
		{
			var endTime = DateTime.Now;
			var totalDuration = endTime - _startTime;
			var passedCount = _results.Count(r => r.Passed);
			var successRate = _results.Count > 0 ? (double)passedCount / _results.Count * 100 : 0;

			_html.AppendLine("</div>"); // Close test-results div

			// Update summary stats
			_html.AppendLine("<script>");
			_html.AppendLine("document.getElementById('summary-stats').innerHTML = `");
			_html.AppendLine($"<p>Total Tests: {_results.Count}</p>");
			_html.AppendLine($"<p>Passed: {passedCount}</p>");
			_html.AppendLine($"<p>Failed: {_results.Count - passedCount}</p>");
			_html.AppendLine($"<p>Success Rate: {successRate:F2}%</p>");
			_html.AppendLine($"<p>Total Duration: {totalDuration.TotalSeconds:F2}s</p>");
			_html.AppendLine("`;");
			_html.AppendLine("</script>");

			_html.AppendLine("</body>");
			_html.AppendLine("</html>");

			Directory.CreateDirectory("TestResults");
			File.WriteAllText("TestResults/report.html", _html.ToString());
			_output.WriteLine($"Report generated at: {Path.GetFullPath("TestResults/report.html")}");
		}

		private class TestResult
		{
			public string Name { get; set; }
			public bool Passed { get; set; }
			public TimeSpan Duration { get; set; }
			public string Message { get; set; }

		}
	}
}