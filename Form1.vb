Imports System.Char
Imports System.Windows.Forms.Application ' for doevents command
Imports Microsoft.VisualBasic.VbStrConv 'for uppercase and lowercase
Public Class Form1

    Dim SymbolTable As New ArrayList 'array for symbol table
    Dim KeyWordTable As String = "#include <conio.h> <stdio.h> <iostream.h> conio.h stdio.h iostream.h void int float main begin end end. char while do" 'keyWord table
    'Dim KeyWordTable As String = "#include <conio.h> <stdio.h> <iostream.h> conio.h stdio.h iostream.h void int float main begin end end. char for to while if then else repeat until elseif do" 'keyWord table
    Dim has_error_in_iexical As Boolean = False
    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        On Error Resume Next
        If txtSourceCode.Text = "" Then
            MsgBox("Write your program first and then compile it.", MsgBoxStyle.Information, "warining")
            Exit Sub
        End If
        has_error_in_iexical = False  'dar ebteda hich errori dar faze avvale compile nist
        optAll.Checked = True 'change option bottun to show all tokens 
        btnSave.Enabled = False
        btnOpen.Enabled = False
        GroupBox4.Enabled = False  'disable view panel
        Button1.Enabled = False 'disable compile  button
        Panel1.Visible = True 'show progress bar panel
        Panel1.Cursor = Cursors.WaitCursor   ' change mouse icon
        SymbolTable.Clear() 'clear symbol table on every compile button click
        txtResult1.Clear() 'clear results

        Dim i As Integer
        Dim sourceString As String
        Dim SourceArray() As String
        sourceString = txtSourceCode.Text
        ReDim SourceArray(sourceString.Length)
        Dim str As String = ""

        For i = 0 To txtSourceCode.Lines.Length - 1
            If txtSourceCode.Lines(i).Contains("//") = True Then 'delete commnet from source code
                Dim index As Integer
                Dim w As Integer
                Dim commentText As String = ""
                Dim str2 As String = ""
                index = InStr(txtSourceCode.Lines(i), "//")
                For w = 0 To index - 2   ' line string  without commnet
                    str = str & txtSourceCode.Lines(i).Chars(w)
                Next w
            Else
                If txtSourceCode.Lines(i) <> "" Then str = str & txtSourceCode.Lines(i).Trim & " " 'remove newline space and add " " to end of each line
            End If

        Next


        ' split string and save to array with " " sep
        ' har onsore SourceArray yek kalame ast
        ' compare method:"CompareMethod.Text" => jostjoo hassas be horoofe koochak va bozorg nist
        SourceArray = Split(str, " ", , CompareMethod.Text)

        ProgressBar1.Value = 0 'start progres sbar
        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = CInt(SourceArray.GetUpperBound(0)) + 2
        For i = 0 To SourceArray.GetUpperBound(0)

            Label4.Text = "Please Wait   / "
            DoEvents()

            Label4.Text = "Please Wait   - "
            DoEvents()

            Label4.Text = "Please Wait   \ "
            DoEvents()

            ProgressBar1.Value = ProgressBar1.Value + 1  ' change value of progress bar

            Lexical(SourceArray(i))  'phase 1 => tahlilgare loghavi (lexical analyser)

        Next
        'TabControl1.Enabled = True
        cmdStartSyntaxAna.Enabled = True  ' enable syntax analyser button
        Panel1.Cursor = Cursors.Default  ' restore mouse pointer to default cursor
        Panel1.Visible = False 'hide progress bar panel
        Button1.Enabled = True  ' enable compile button
        btnSave.Enabled = True
        btnOpen.Enabled = True
        GroupBox4.Enabled = True


    End Sub  'compile click in lexical analyser section

    Private Sub Lexical(ByVal word As String)

        'line space
        If word = "" Or word = " " Then
            Exit Sub
        End If

        'check key words
        If is_keyword(word) = True Then
            add_to_Symbol_Table(word, "Keyword", "-")
            Exit Sub
        End If

        'check rel opetation < , <= , > , >= , =
        Dim Attribute As String = ""
        If Is_RelOp(word, Attribute) = True Then
            add_to_Symbol_Table(word, "RelOP", Attribute)
            Exit Sub
        End If

        'check is number or no
        If IsNumeric(word) Then
            add_to_Symbol_Table(word, "num", SymbolTable.Count)
            Exit Sub
        End If

        'check shenaseh ( variable)
        If Is_Shenaseh(word) = True Then
            add_to_Symbol_Table(word, "id", SymbolTable.Count)
            Exit Sub
        End If

        'check math operations ex: + , - , * , / , ^
        Dim Attribute3 As String = ""
        If Is_math_op(word, Attribute3) = True Then
            add_to_Symbol_Table(word, "mathOp", Attribute3)
            Exit Sub
        End If

        'check parantes ex: ( , )
        Dim attribute4 As String = ""
        If is_parantess(word, Attribute4) = True Then
            add_to_Symbol_Table(word, "Parantes", Attribute4)
            Exit Sub
        End If

        If is_endmarker(word) = True Then
            add_to_Symbol_Table(word, "EndMarker", SymbolTable.Count)
            Exit Sub
        End If

        'other symbols ( error )
        add_to_Symbol_Table(word, "UnKnown", SymbolTable.Count)

    End Sub 'lexical analyser 

    Private Function Is_Shenaseh(ByVal wrd As String) As Boolean
        ' nahveye tarife shenaseh
        '1- hade aksar 4 harf dashte bashad
        '2-mitavanad ba adad shoro shavad va ya ba harf
        '3-hade aghal shamele yek harf bashad
        '4-char haye morede estefade  harf , adad
        Dim currentChar As Char
        Dim i As Integer
        If wrd.Length > 4 Then
            Is_Shenaseh = False
            Exit Function
        End If

        If wrd.Length <= 4 Then

            If IsNumeric(wrd) = True Then ' adady ast
                Is_Shenaseh = False
                Exit Function
            End If

            For i = 1 To wrd.Length
                currentChar = Mid(wrd, i, 1)
                If is_Digit_or_alphabet(currentChar) = False Then
                    Is_Shenaseh = False
                    Exit Function
                End If
            Next
            Is_Shenaseh = True
            Exit Function
        End If

    End Function 'is shenaseh function

    Private Function Is_RelOp(ByVal wrd As String, ByRef attr As String) As Boolean
        If wrd.Length > 2 Then
            Is_RelOp = False
            Exit Function
        End If
        If wrd.Length <= 2 Then
            Select Case wrd
                Case "<"
                    attr = "LT"
                    Is_RelOp = True 'LT
                    Exit Function
                Case "<="
                    attr = "LE"
                    Is_RelOp = True 'LE
                    Exit Function
                Case ">"
                    attr = "GT"
                    Is_RelOp = True 'GT
                    Exit Function
                Case ">="
                    attr = "GE"
                    Is_RelOp = True 'GE
                    Exit Function
                Case "="
                    attr = "EQ"
                    Is_RelOp = True  'EQ
                    Exit Function
                Case Else
                    Is_RelOp = False
                    Exit Function
            End Select

        End If



    End Function ' is relOp function

    Private Function is_Digit_or_alphabet(ByVal ch As Char) As Boolean

        Dim str As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" 'charachter haye mojaz baraye adad va harf
        If InStr(str, ch) <> 0 Then ' search "ch" in str 
            is_Digit_or_alphabet = True
            Exit Function
        Else
            is_Digit_or_alphabet = False
            Exit Function
        End If
    End Function  ' is digit of letter function

    Private Function is_keyword(ByVal wrd As String) As Boolean

        Dim k() As String
        ReDim k(KeyWordTable.Length)
        k = Split(KeyWordTable, " ")
        Dim i As Integer

        For i = 0 To k.GetUpperBound(0)
            If k(i) = wrd Then
                is_keyword = True
                Exit Function
            End If
        Next i

        is_keyword = False
        Exit Function


    End Function ' is keyword function

    Private Function Is_math_op(ByVal wrd As String, ByRef attr As String) As Boolean
        If wrd.Length > 1 Then
            Is_math_op = False
            Exit Function
        End If

        If wrd.Length = 1 Then
            Select Case wrd
                Case "+"
                    attr = "sum"
                    Is_math_op = True
                    Exit Function
                Case "-"
                    attr = "min"
                    Is_math_op = True
                    Exit Function
                Case "*"
                    attr = "mul"
                    Is_math_op = True
                    Exit Function
                Case "/"
                    attr = "div"
                    Is_math_op = True
                    Exit Function
                Case "^"
                    attr = "pow"
                    Is_math_op = True  'EQ
                    Exit Function
                Case Else
                    Is_math_op = False
                    Exit Function
            End Select

        End If
    End Function ' is math op function

    Private Function is_endmarker(ByVal ch As String) As Boolean
        If ch.Length = 1 Then
            If ch = ";" Then
                is_endmarker = True
                Exit Function
            Else
                is_endmarker = False
                Exit Function
            End If
        End If
        is_endmarker = False
    End Function ' is ; or no

    Private Sub showOutput(ByVal wrd As String, ByVal token As String, ByVal attr As String)
        txtResult1.Text = txtResult1.Text & " ( " & wrd & " , " & token & " , " & attr & " ) " & vbCrLf
    End Sub 'show result 

    Private Sub add_to_Symbol_Table(ByVal word As String, ByVal token As String, ByVal attr As String)
        SymbolTable.Add(word) ' add to array list ( symbol table )
        SymbolTable.Add(token) ' add to array list ( symbol table )
        SymbolTable.Add(attr) ' add to array list ( symbol table )
        showOutput(word, token, attr) ' show result
    End Sub ' add to symbol table 

    Private Function is_parantess(ByVal wrd As String, ByRef attr As String) As Boolean
        If wrd.Length <> 1 Then
            is_parantess = False
            Exit Function
        End If
        If wrd.Length = 1 Then
            Select Case wrd
                Case "("
                    attr = "POpen"
                    is_parantess = True
                    Exit Function
                Case ")"
                    attr = "PClose"
                    is_parantess = True
                    Exit Function
                Case Else
                    is_parantess = False
                    Exit Function
            End Select

        End If
    End Function ' is parantes function

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        ' save code event
        Dim strFileName As String
        If (SaveFileDialog1.ShowDialog() = vbOK) Then
            strFileName = SaveFileDialog1.FileName
            System.IO.File.WriteAllText(strFileName, txtSourceCode.Text)
        End If
    End Sub  'save code routin

    Private Sub btnOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen.Click
        'open code event
        Dim strFileName As String
        If (OpenFileDialog1.ShowDialog = vbOK) Then
            strFileName = OpenFileDialog1.FileName
            txtSourceCode.Text = System.IO.File.ReadAllText(strFileName)
        End If
    End Sub  'open code routin

    Private Sub optErrors_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optErrors.CheckedChanged
        'filter result by unknown (errors)
        If SymbolTable.Count <> 0 Then
            txtResult1.Clear()
            Dim i As Long

            For i = 0 To SymbolTable.Count - 1 Step 3
                If SymbolTable(i + 1) = "UnKnown" Then
                    has_error_in_iexical = True  'set global flag ( marhaleye tahlilgare loghavi error darad)
                    showOutput(SymbolTable(i), SymbolTable(i + 1), SymbolTable(i + 2))
                End If
            Next
        End If
    End Sub 'show errors results 

    Private Sub optAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optAll.CheckedChanged
        'show all results
        If SymbolTable.Count <> 0 Then
            txtResult1.Clear()
            Dim i As Long

            For i = 0 To SymbolTable.Count - 1 Step 3
                showOutput(SymbolTable(i), SymbolTable(i + 1), SymbolTable(i + 2))
            Next
        End If
    End Sub 'show all results

    Private Sub optCorrect_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optCorrect.CheckedChanged
        'show correct (known) result
        If SymbolTable.Count <> 0 Then
            txtResult1.Clear()
            Dim i As Long

            For i = 0 To SymbolTable.Count - 1 Step 3
                If SymbolTable(i + 1) <> "UnKnown" Then
                    showOutput(SymbolTable(i), SymbolTable(i + 1), SymbolTable(i + 2))
                End If
            Next
        End If
    End Sub 'show correct results

    '==========================================================================================
    '===============================  syntax analyser code section  ===========================
    '==========================================================================================

    Private Sub cmdStartSyntaxAna_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStartSyntaxAna.Click
        txtSyntaxOutput.Clear() 'clear syntax analyser results

        If has_error_in_iexical = True Then ' check mikonad agar dar faze avval error bashad ebteda bayad anra tashih kard
            MsgBox("Please correct errors in lexical analyser phase.", MsgBoxStyle.Exclamation, "Warning")
            Exit Sub
        End If

        '=================================================================================================
        'start syntax analyser routin
        '=================================================================================================
        Check_includes()  'checke vojoode hade aghal yeki az include ha [ "#incule <conio.h>" or "#inlclude <stdio.h>" or "#include <iostream.h> ]"
        check_main_program() 'check "main" and "begin" and "end." for main   void|int|float main ( void | int | float ) begin .... end.
        Is_symmetric_Parantes() 'checke tagharone parantes ha
        Is_symmetric_Begin_end() 'checke tagharone "begin" and "end" or "end."
        check_While_syntax() 'checke sakhtare while
        check_entesab() 'check kardane ebarate entesab
        check_variable_def() 'checke tarife motaghayyer
        '=================================================================================================
        'end if syntax analyser routin
        '=================================================================================================
    End Sub

    Private Function Check_includes() As Boolean

        'checke vojode hade aghal yeki az dastoorate zir
        '#include <conio.h>  
        '#include <stdio.h>  
        '#include <iostream.h>  

        If SymbolTable(0).ToUpper() = "#INCLUDE" Then
            If SymbolTable(3).ToUpper() = "<CONIO.H>" Or SymbolTable(3).ToUpper() = "<STDIO.H>" Or SymbolTable(3).ToUpper() = "<IOSTREAM.H>" Then
                Check_includes = True
                Exit Function
            End If
        End If
        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  ""#include"" error. please check ""#include <conio.h>"" , ""#include <stdio.h>"" , ""#include <iostream.h>""  at first of program." & vbCrLf
        Check_includes = False
    End Function 'checke vojoode hade aghal yeki az include ha

    Private Function check_main_program() As Boolean
        Dim i As Integer
        Dim MainExistFlag As Boolean = False 'farz bar in ast ke main vojood nadarad
        'checke vojoode main dar barnameh
        For i = 0 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i).ToUpper() = "MAIN" Then
                MainExistFlag = True
            End If
        Next

        If MainExistFlag = False Then
            txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Main of program not found." & vbCrLf
            Exit Function
        End If

        For i = 0 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i).ToUpper() = "MAIN" Then
                If SymbolTable(i - 3).ToUpper() = "VOID" Or SymbolTable(i - 3).ToUpper() = "INT" Or SymbolTable(i - 3).ToUpper() = "FLOAT" Then
                    If SymbolTable(i + 3).ToUpper() = "(" Then
                        If SymbolTable(i + 6).ToUpper() = "VOID" Or SymbolTable(i + 6).ToUpper() = "INT" Or SymbolTable(i + 6).ToUpper() = "FLOAT" Then
                            If SymbolTable(i + 9).ToUpper() = ")" Then
                                If SymbolTable(i + 12).ToUpper() = "BEGIN" Then
                                    If SymbolTable(SymbolTable.Count - 3).ToUpper() = "END." Then
                                        check_main_program = True
                                        Exit Function
                                    Else
                                        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  ""End."" for ""main"" not exist." & vbCrLf
                                        check_main_program = False
                                        Exit Function
                                    End If

                                Else
                                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  ""begin"" for ""main"" not exist." & vbCrLf
                                    check_main_program = False
                                    Exit Function
                                End If
                            End If
                        Else ' 'not void-int-float after main
                            txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  not found ""void"" or ""int"" or ""float"" after ""main""." & vbCrLf
                            check_main_program = False
                            Exit Function

                        End If
                    Else ' not found (
                        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Not found ""("" after main." & vbCrLf
                    End If
                Else 'not void-int-float before main
                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  not found ""void"" or ""int"" or ""float"" before ""main""." & vbCrLf
                    check_main_program = False
                    Exit Function

                End If
            End If
        Next

        check_main_program = False
    End Function 'check "main" and "begin" and "end." for main   void|int|float main ( void | int | float ) begin .... end.

    Private Function Is_symmetric_Parantes() As Boolean
        Dim i As Long
        Dim count As Integer = 0
        For i = 0 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i + 2).ToUpper() = "POPEN" Then
                count = count + 1
            ElseIf SymbolTable(i + 2).ToUpper() = "PCLOSE" Then
                count = count - 1
            End If
        Next


        If count = 0 Then
            Is_symmetric_Parantes = True
            Exit Function
        ElseIf count < 0 Then 'tedade parantes baste bishtar ast
            txtSyntaxOutput.Text = txtSyntaxOutput.Text & " ""("" expected." & "  ( " & (-1 * count) & " parantes)" & vbCrLf '  "* tedade ""begin"" " & (-1 * count) & " ta bishtar ast." & vbCrLf
            Is_symmetric_Parantes = False
            Exit Function
        ElseIf count > 0 Then 'tedade parantes baz bishtar ast
            txtSyntaxOutput.Text = txtSyntaxOutput.Text & " "")"" expected." & "  ( " & count & " parantes)" & vbCrLf
            Is_symmetric_Parantes = False
            Exit Function
        End If

    End Function 'checke tagharone parantes ha

    Private Function Is_symmetric_Begin_end() As Boolean
        Dim i As Long
        Dim count As Integer = 0
        For i = 0 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i).ToUpper() = "BEGIN" Then
                count = count + 1
            ElseIf SymbolTable(i).ToUpper() = "END" Then
                count = count - 1
            ElseIf SymbolTable(i).ToUpper() = "END." Then
                count = count - 1

            End If

        Next


        If count = 0 Then
            Is_symmetric_Begin_end = True
            Exit Function
        ElseIf count < 0 Then
            txtSyntaxOutput.Text = txtSyntaxOutput.Text & " " * "begin"" expected." & "(" & (-1 * count) & ")" & vbCrLf '  "* tedade ""begin"" " & (-1 * count) & " ta bishtar ast." & vbCrLf
            Is_symmetric_Begin_end = False
            Exit Function
        ElseIf count > 0 Then
            txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  ""end"" expected." & "(" & count & ")" & vbCrLf
            Is_symmetric_Begin_end = False
            Exit Function
        End If

    End Function 'checke tagharone "begin" and "end" or "end."

    Private Function check_While_syntax() As Boolean
        'While ( I R X )
        '   //statement
        '[while|-|-] [(|parantes|POpen] [? | id or num | - ] [-|RelOp|-] [-|id or num | - ] [-|parantes|PClose]
        '  i                      i+5          i+7               i+10          i+13                       i+17
        Dim i As Long
        Dim attr As String = ""
        For i = 0 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i).ToUpper() = "WHILE" Then
                If SymbolTable(i + 5).ToUpper() = "POPEN" Then
                    If (SymbolTable(i + 7)) = "id" Or (SymbolTable(i + 7) = "num") Then
                        If SymbolTable(i + 10).ToUpper() = "RELOP" Then
                            If (SymbolTable(i + 13)) = "id" Or (SymbolTable(i + 13) = "num") Then
                                If SymbolTable(i + 17).ToUpper() = "PCLOSE" Then
                                    check_While_syntax = True
                                    'Exit Function
                                Else
                                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  "")"" not found in ""While"" command.please check it." & vbCrLf
                                    check_While_syntax = False
                                    Exit Function
                                End If
                            Else
                                txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  " & SymbolTable(i + 12) & " not number or variable in ""while"" command." & vbCrLf
                                check_While_syntax = False
                                Exit Function
                            End If
                        Else
                            txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  " & SymbolTable(i + 9) & " not Rel Operation in ""while"" command." & vbCrLf
                            check_While_syntax = False
                            Exit Function
                        End If
                    Else
                        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  " & SymbolTable(i + 6) & " not number or variable in ""while"" command." & vbCrLf
                        check_While_syntax = False
                        Exit Function
                    End If
                Else
                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  ""("" not found in ""While"" command.please check it." & vbCrLf
                    check_While_syntax = False
                    Exit Function
                End If
            End If
        Next
        check_While_syntax = False
    End Function 'checke sakhtare while

    Private Function check_entesab() As Boolean
        
        'id=id;  id=variable
        '[abcd|id|arrtibute] [=|RelOP|EQ] [word|id|arrtibute] [;|EndMarker|arrtibute]

        'id=id;  id=number
        '[abcd|id|arrtibute] [=| RelOP|EQ] [1245|num|arrtibute] [;|EndMarker|arrtibute]
        '  0    1    2        3    4    5     6   7    8         9    10        11 
        ' i-5  i-4  i-3       i-2  i-1  i    i+1  i+2  i+3       i+4  i+5        i+6  

        Dim i As Long
        For i = 2 To SymbolTable.Count - 1 Step 3
            If SymbolTable(i) = "EQ" Then '= operator
                If SymbolTable(i - 4) = "id" Then 'variable
                    If (SymbolTable(i + 2) = "num") Or (SymbolTable(i + 2) = "id") Then 'number
                        If SymbolTable(i + 5) = "EndMarker" Then ';
                            check_entesab = True
                        Else
                            txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  "";"" not found. [ " & SymbolTable(i - 5) & " " & SymbolTable(i - 2) & " " & SymbolTable(i + 1) & "<<< ]" & vbCrLf
                        End If
                    Else
                        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Right of ""="" operator must be number or variable. [ " & SymbolTable(i - 5) & " " & SymbolTable(i - 2) & " " & SymbolTable(i + 1) & "<<< ]" & vbCrLf
                    End If
                Else
                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Left of ""="" operator must variable. [ " & SymbolTable(i - 5) & "<<< " & SymbolTable(i - 2) & " " & SymbolTable(i + 1) & " ]" & vbCrLf
                End If
            End If
        Next
    End Function 'check kardane entesab dar barnameh

    Private Function check_variable_def() As Boolean
        '(int|float|char) varName ;

        '[int|Keyword|-] [abcd|id|arrtibute] [;|EndMarker|arrtibute]
        '[float|Keyword|-] [abcd|id|arrtibute] [;|EndMarker|arrtibute]
        '[char | Keyword |- ]  [abcd | id | arrtibute] [;|EndMarker|arrtibute]
        '  i      i+1    i+2    i+3    i+4    i+5       i+6  i+7      i+8

        Dim i As Long
        For i = 0 To SymbolTable.Count - 1 Step 3
            If (SymbolTable(i) = "int") Or (SymbolTable(i) = "float") Or (SymbolTable(i) = "char") Then
                If SymbolTable(i + 3).ToUpper() = "MAIN" Then GoTo continue1 'agar main bood yani tarife motaghayyer nist  masalan[ int main (int)]
                If (SymbolTable(i - 1).ToUpper() = "POPEN") And (SymbolTable(i + 5).ToUpper() = "PCLOSE") Then GoTo continue1 ' ghabl ba bad az "int" "float" "char" agar parantez bood yani tarife motaghayyer nist. masalan "void main(int)"
                If SymbolTable(i + 4) = "id" Then  'name motaghayyer bayad dorost bashad
                    If (SymbolTable(i + 7) = "EndMarker") Then 'akhare tarife motaghayyer ; bayad bashad
                        check_variable_def = True
                    Else
                        txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  "";"" not found in variable definition.  [ " & SymbolTable(i) & " " & SymbolTable(i + 3) & "<<  ]" & vbCrLf
                    End If
                Else
                    txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Variable name error. [ " & SymbolTable(i) & " " & SymbolTable(i + 3) & "<< " & SymbolTable(i + 6) & " ]" & vbCrLf
                End If
            Else
                'txtSyntaxOutput.Text = txtSyntaxOutput.Text & "*  Type of variable muse be (int|float|char).[ " & SymbolTable(i) & "<< " & SymbolTable(i + 3) & " " & SymbolTable(i + 6) & " ]" & vbCrLf
            End If
continue1:
        Next
    End Function 'checke tarife motaghayyer

End Class
