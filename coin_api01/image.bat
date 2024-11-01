@echo off
SET YYYY=%date:~0,4%
SET YY=%date:~2,2%
SET MM=%date:~5,2%
SET DD=%date:~8,2%
SET hour=%time:~0,2%
SET min=%time:~3,2%
SET sec=%time:~6,2%

if %MM% == 01 (
 SET MOSCODE=A
) else if %MM% == 02 (
 SET MOSCODE=B
) else if %MM% == 03 (
 SET MOSCODE=C
) else if %MM% == 04 (
 SET MOSCODE=D
) else if %MM% == 05 (
 SET MOSCODE=E
) else if %MM% == 06 (
 SET MOSCODE=F
) else if %MM% == 07 (
 SET MOSCODE=G
) else if %MM% == 08 (
 SET MOSCODE=H
) else if %MM% == 09 (
 SET MOSCODE=I
) else if %MM% == 10 (
 SET MOSCODE=J
) else if %MM% == 11 (
 SET MOSCODE=K
) else if %MM% == 12 (
 SET MOSCODE=L
) else (
 SET MOSCODE=Z
)
echo on

set filename=coin-api01
set filever=%YY%%MOSCODE%%DD%%min%%sec%
set fullfilename=%filename%:%filever%
set repository=peter0121/

docker build -f Dockerfile -t %repository%%fullfilename% ..