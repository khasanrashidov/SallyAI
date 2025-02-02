using OfficeOpenXml;
using System.Text.Json;

namespace OpenAiSample.WebApi.Services.JsonToExcel
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcelFromJson(JsonElement jsonElement)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PDW_Infrastructure");

                int row = 1; // Start from the first row

                // Access and write the functional areas
                var functionalAreas = jsonElement.GetProperty("ProjectDiscoveryWorkshopReport").GetProperty("functionalAreas");

                // Before iterating over the areas, write the headers with custom colors
                worksheet.Cells[row, 1].Value = "Area";
                worksheet.Cells[row, 2].Value = "Feature";
                worksheet.Cells[row, 3].Value = "Description";
                worksheet.Cells[row, 4].Value = "Time (days)";

                // Set header background color
                using (var range = worksheet.Cells[row, 1, row, 4])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.Font.Bold = true;
                }

                row++; // Move to the next row after the header

                // Iterate over the functional areas and add colors to cells
                foreach (JsonProperty area in functionalAreas.EnumerateObject())
                {
                    worksheet.Cells[row, 1].Value = area.Name; // Write the area name (Infrastructure, Backend, etc.)
                    worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Area color

                    foreach (JsonElement feature in area.Value.GetProperty("features").EnumerateArray())
                    {
                        worksheet.Cells[row, 2].Value = feature.GetProperty("name").GetString(); // Write the feature name
                        worksheet.Cells[row, 3].Value = feature.GetProperty("description").GetString(); // Write the description
                        worksheet.Cells[row, 4].Value = feature.GetProperty("time").GetInt32(); // Write the time

                        // Optional: Apply colors to feature cells
                        worksheet.Cells[row, 2, row, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2, row, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow); // Feature cells color

                        row++; // Move to the next row
                    }
                }

                row += 5; // Add some space between functional areas
                // Add general information about the team structure
                var teamStructure = jsonElement.GetProperty("ProjectDiscoveryWorkshopReport").GetProperty("teamStructure");

                worksheet.Cells[row, 1].Value = "Team Structure";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                row++;

                foreach (JsonElement role in teamStructure.GetProperty("roles").EnumerateArray())
                {
                    worksheet.Cells[row, 1].Value = role.GetProperty("name").GetString(); // Write the role name
                    worksheet.Cells[row, 2].Value = role.GetProperty("responsibilities").GetString(); // Write the responsibilities
                    worksheet.Cells[row, 3].Value = role.GetProperty("stack").GetString(); // Write the stack

                    // Optional: Apply colors to role cells
                    worksheet.Cells[row, 1, row, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen); // Role cells color

                    row++; // Move to the next row
                }

                // Autofit columns based on content
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }

        public byte[] GenerateRoadmapExcel(JsonElement jsonElement)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Roadmap");

                int row = 1;

                // Write the headers
                worksheet.Cells[row, 1].Value = "Quarter";
                worksheet.Cells[row, 2].Value = "Major Implementations";
                worksheet.Cells[row, 3].Value = "Minor Implementations";
                worksheet.Cells[row, 4].Value = "Continuous Efforts";

                using (var range = worksheet.Cells[row, 1, row, 4])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    range.Style.Font.Bold = true;
                }

                row++;

                // Access and iterate over the Roadmap
                var roadmap = jsonElement.GetProperty("Roadmap");

                foreach (JsonProperty quarter in roadmap.EnumerateObject())
                {
                    // Write the quarter (Q1, Q2, etc.)
                    worksheet.Cells[row, 1].Value = quarter.Name;
                    worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Quarter cell color

                    // Write Major Implementations
                    var majorImplementations = quarter.Value.GetProperty("majorImplementations").EnumerateArray();
                    worksheet.Cells[row, 2].Value = string.Join(", ", majorImplementations.Select(imp => imp.GetString()));

                    // Write Minor Implementations
                    var minorImplementations = quarter.Value.GetProperty("minorImplementations").EnumerateArray();
                    worksheet.Cells[row, 3].Value = string.Join(", ", minorImplementations.Select(imp => imp.GetString()));

                    // Write Continuous Efforts
                    var continuousEfforts = quarter.Value.GetProperty("continuousEfforts").EnumerateArray();
                    worksheet.Cells[row, 4].Value = string.Join(", ", continuousEfforts.Select(imp => imp.GetString()));

                    row++;
                }

                // Autofit columns based on content
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }
    }
}
