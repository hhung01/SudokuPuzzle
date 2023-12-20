'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Namespace Model

    Friend Enum CellStateEnum
        _min = 0
        Blank = _min
        Notes
        UserInput
        Hint
        Answer
        _max = Answer
    End Enum

End Namespace
