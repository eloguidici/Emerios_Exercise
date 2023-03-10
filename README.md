# README
[![N|Solid](https://www.emerios.com/hubfs/New%20EES%20Logo_white.svg)](https://www.emerios.com/)

## CONTENTS OF THIS FILE
   
* Introduction
* Requirements
* User Story
* Installation

## INTRODUCTION

Challenge: "Find the longest substring present in an array"

## REQUIREMENTS

It needs to have Visual Studio installed.

## USER STORY

Let be a two-dimensional square array of (any) characters, return the longest string of equal adjacent characters.

```bash
                                        B, B, D, A, D, E, F
                                        B, X, C, D, D, J, K
                                        H, Y, I, 3, D, D, 3
                                        R, 7, O, N, G, D, 2
                                        W, N, S, P, E, 0, D
                                        A, 9, C, D, D, E, F
                                        B, X, D, D, D, J, K
```

It should return the string D, D, D, D, D, because there is a slash of D of length 5 which is the longest.


## INSTALLATION

Open the Terminal un Visual Studio (cmd.exe).
Navigate to the folder where your C# project is located using the "cd" command followed by the folder path.

Run the following command to compile your project and generate the executable file:
```bash
csc /out:AppMatrix.exe Program.cs
```
This will generate an executable file in the same folder. (AppMatrix.exe)

**There is an executable file in the root of the project.**

## EXTRAS
- Author - [Emiliano Loguidici](https://www.linkedin.com/in/emilianologuidici/)
