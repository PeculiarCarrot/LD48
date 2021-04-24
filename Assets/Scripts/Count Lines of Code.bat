powershell -Command "dir -Recurse -Include *.cs | Get-Content | Measure-Object -Line"
pause