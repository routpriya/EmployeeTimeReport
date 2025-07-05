Employee Time Report

This C# console application fetches employee time entries from an API, calculates the total time worked by each employee, and generates an HTML report. 

Features:
- Fetches data from the live API
- Groups time entries by employee
- Generates an HTML report showing total hours worked for each employee
- Highlights employees who have worked less than 100 hours with a red background

How to Run:
1.Clone the repository:
   ```bash
   git clone https://github.com/routpriya/EmployeeTimeReport.git
````
2. Navigate to the folder:
   ```bash
   cd EmployeeTimeReport
   ```
3. Run the application:
   Make sure you have .NET Coreinstalled on your machine (any version from 6.0 or higher)
   Run the following command: 
   dotnet run
4. View the Report:
The HTML report will be generated in the same folder and named `output.html`. This will automatically open in your default browser.
If you don’t see the report immediately, you can manually open `output.html`.

Technologies Used:
* C#
* .NET Core
* API Integration (JSON data)
* HTML for report generation
