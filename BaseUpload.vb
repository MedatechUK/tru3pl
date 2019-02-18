Imports System.Data.SqlClient
Imports System.IO

Public MustInherit Class Upload : Inherits Dictionary(Of String, Integer) : Implements IDisposable

#Region "Local Variables"

    Private CreateDate As Integer = DateDiff(DateInterval.Minute, #1/1/1988#, Now)
    Private showProgress As Boolean = False
    Private Cur As cursorloc

#End Region

#Region "overridable Properties"

    Overridable ReadOnly Property FileName As String

    Overridable ReadOnly Property cmd As SqlCommand
        Get
            Return Nothing
        End Get
    End Property

    Overridable ReadOnly Property rowcount As SqlCommand
        Get
            Return Nothing
        End Get
    End Property

    Overridable ReadOnly Property update(ParamArray keys() As Integer) As SqlCommand
        Get
            Return Nothing
        End Get
    End Property

#End Region

#Region "Properties"

    Public ReadOnly Property FileStr As String
        Get
            Dim ret As String = String.Format("{0}{1}.txt", FileName, CreateDate)
            Return ret
        End Get
    End Property

#End Region

#Region "Methods"

    Public Sub ProgressBar()
        showProgress = True
        args.Log("Writing file {0}", FileStr)
        args.line("Writing file {0}", FileStr)
        Cur = New cursorloc(rowcount.ExecuteScalar())

    End Sub

    Public Function CreateMap(r As SqlDataReader) As Dictionary(Of Integer, Integer)
        Dim ret As New Dictionary(Of Integer, Integer)
        For i As Integer = 0 To r.FieldCount - 1
            If Keys.Contains(r.GetName(i)) Then
                ret.Add(Me(r.GetName(i)), i)

            End If

        Next
        Return ret

    End Function

    Public Sub write(ByRef sw As StreamWriter, ByVal map As Dictionary(Of Integer, Integer), ByVal r As SqlDataReader)

        With sw
            For i As Integer = 0 To Count - 1
                If map.Keys.Contains(i) Then
                    If Not IsDBNull(r(map(i))) Then
                        sw.Write(String.Format("{0}{1}{0}", Chr(34), Replace(r(map(i)), Chr(34), "''")))
                    End If
                End If

                sw.Write(",")

            Next
            sw.Write(sw.NewLine)

        End With

        If showProgress Then Cur.current += 1

    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
