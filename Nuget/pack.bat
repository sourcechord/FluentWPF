set CurrentDirectory=%CD%
pushd ..\FluentWPF
dotnet pack -c Release -o %CurrentDirectory%
popd
pause
