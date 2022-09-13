﻿Imports System.IO
'========================
'========= NODE =========
'========================
Public MustInherit Class Node


    Public positionStart As Integer
    Public positionEnd As Integer
    Public parentNode As Node = Nothing

    Public Sub New(ByVal positionStart As Integer, ByVal positionEnd As Integer)
        Me.positionStart = positionStart
        Me.positionEnd = positionEnd
    End Sub

    Public Overrides Function ToString() As String
        Return "()"
    End Function

End Class

'==================================
'========= CONTAINER NODE =========
'==================================
Public MustInherit Class containerNode
    Inherits Node

    Public variables As New List(Of Variable)
    Public codes As New List(Of Node)

    Public Sub New(ByVal positionStart As Integer, ByVal positionEnd As Integer)
        MyBase.New(positionStart, positionEnd)
    End Sub

    Public Sub addNodeToCode(ByVal node As Node)
        node.parentNode = Me
        codes.Add(node)
    End Sub

End Class

'========================
'========= FILE =========
'========================
Public Class FileNode
    Inherits Node

    'Variables
    Public text As String
    Public name As String
    Public path As String

    Public spaces As List(Of SpaceNode)

    'New
    Public Sub New(ByVal path As String)

        'Inherits
        MyBase.New(0, 0)

        'Load infos
        Me.text = text
        path = path.Replace("\", "/")
        Me.path = path
        If path.Contains("/") Then
            Me.name = path.Substring(path.LastIndexOf("/"))
        Else
            Me.name = path
        End If

        'Read file
        Try
            Me.text = File.ReadAllText(path)
        Catch ex As Exception
            addCustomError("Unable to read", ex.Message)
        End Try

        'Generates Tokens
        Dim lexer As New lexer()
        Dim tokens As List(Of token) = lexer.parse(Me.text, Me.name)

        'Generate AST
        Dim parser As New nodeParser()
        spaces = parser.parse(tokens, text, Me.name)

        'Set parents
        For i As Integer = 0 To spaces.Count - 1
            spaces(i).parentNode = Me
        Next

    End Sub

    'ToString
    Public Overrides Function ToString() As String
        Return "file " & name
    End Function

End Class

'=========================
'========= SPACE =========
'=========================
Public Class SpaceNode
    Inherits containerNode

    'Variables
    Public name As String

    'New
    Public Sub New(ByVal positionStart As Integer, ByVal positionEnd As Integer, ByVal name As String)
        MyBase.New(positionStart, positionEnd)
        Me.name = name
    End Sub

    'ToString
    Public Overrides Function ToString() As String
        Dim content As String = ""
        For Each code As Node In Me.codes
            content &= Environment.NewLine & vbTab & code.ToString()
        Next
        Return "space " & name & "" & content
    End Function

End Class