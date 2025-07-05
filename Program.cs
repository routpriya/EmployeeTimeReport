using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

class Program
{
    class TimeEntry
    {
        public string? EmployeeName { get; set; }
        public string? StarTimeUtc { get; set; }
        public string? EndTimeUtc { get; set; }
    }

    class EmployeeTotal
    {
        public string EmployeeName { get; set; } = "";
        public double TotalHours { get; set; }
    }

    static async System.Threading.Tasks.Task Main()
    {
        Console.WriteLine("⏳ Fetching data from API...");

        var client = new HttpClient();
        string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
        string json = await client.GetStringAsync(url);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var entries = JsonSerializer.Deserialize<List<TimeEntry>>(json, options);

        if (entries == null || entries.Count == 0)
        {
            Console.WriteLine("❌ No entries found.");
            return;
        }

        var employeeGroups = new Dictionary<string, double>();

        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.EmployeeName)) continue;

            if (DateTime.TryParse(entry.StarTimeUtc, null, DateTimeStyles.AdjustToUniversal, out var start) &&
                DateTime.TryParse(entry.EndTimeUtc, null, DateTimeStyles.AdjustToUniversal, out var end))
            {
                double hours = (end - start).TotalHours;
                if (hours < 0) hours = 0;

                if (employeeGroups.ContainsKey(entry.EmployeeName))
                    employeeGroups[entry.EmployeeName] += hours;
                else
                    employeeGroups[entry.EmployeeName] = hours;
            }
        }

        var employees = employeeGroups
            .Select(e => new EmployeeTotal
            {
                EmployeeName = e.Key,
                TotalHours = Math.Round(e.Value, 2)
            })
            .OrderByDescending(e => e.TotalHours)
            .ToList(); // ✅ No .Take(10) — shows all

        string html = @"<html><head><style>
        body { font-family: Arial; background-color: #ffffff; padding: 20px; }
        table { width: 90%; border-collapse: collapse; margin: auto; }
        th, td { border: 1px solid #ddd; padding: 10px; text-align: left; }
        th { background-color: #4CAF50; color: white; }
        .low-hours { background-color: #ff4d4d; color: white; font-weight: bold; }
        </style></head><body>
        <h2 style='text-align:center;'>Employee Time Report (Grouped)</h2>
        <p style='text-align:center;'>Total Employees: " + employees.Count + @"</p>
        <table><tr><th>Employee Name</th><th>Total Time Worked (hrs)</th></tr>";

        foreach (var emp in employees)
        {
            string rowClass = emp.TotalHours < 100 ? " class='low-hours'" : "";
            html += $"<tr{rowClass}><td>{emp.EmployeeName}</td><td>{emp.TotalHours:F2}</td></tr>";
        }

        html += "</table></body></html>";

        File.WriteAllText("output.html", html);

        Console.WriteLine($"\n✅ Processed {employees.Count} employees.");
        Console.WriteLine("✅ HTML file 'output.html' created.");

        Process.Start(new ProcessStartInfo
        {
            FileName = Path.GetFullPath("output.html"),
            UseShellExecute = true
        });
    }
}
