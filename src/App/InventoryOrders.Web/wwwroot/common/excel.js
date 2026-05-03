function exportTableToExcel(tableId, fileName) {

    try {

        if (!tableId) {
            throw new Error("Table ID is required.");
        }

        var table = document.getElementById(tableId);

        if (!table) {
            throw new Error("Table not found: " + tableId);
        }

        fileName = fileName || "Export.xlsx";

        var workbook = XLSX.utils.table_to_book(table, {
            sheet: "Sheet1"
        });

        XLSX.writeFile(workbook, fileName);

    } catch (error) {

        console.error("Excel Export Error:", error);

        alert("Something went wrong while exporting Excel.");
    }
}


//<button onclick="exportTableToExcel('employeeTable','Employees.xlsx')">
//    Download Excel
//</button>