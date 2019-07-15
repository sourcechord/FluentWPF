set OutDir=%CD%
pushd ..\FluentWPF
dotnet pack -c Release -o %OutDir%
popd
pause
