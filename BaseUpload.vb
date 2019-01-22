Imports System.Data.SqlClient
Imports System.IO

Public MustInherit Class Upload : Inherits Dictionary(Of String, Integer) : Implements IDisposable

    MustOverride ReadOnly Property FileName As String

    Private sw As StreamWriter
    Sub New()
        sw = New StreamWriter(Path.Combine("d:", ThisFileName))

    End Sub

    Private ReadOnly Property ThisFileName As String
        Get
            Return String.Format("{0}{1}.txt", FileName, DateDiff(DateInterval.Minute, #1/1/1988#, Now))
        End Get
    End Property

    Public ReadOnly map As New Dictionary(Of Integer, Integer)
    Public Sub CreateMap(r As SqlDataReader)

        For i As Integer = 0 To r.FieldCount - 1
            If Keys.Contains(r.GetName(i)) Then
                map.Add(Me(r.GetName(i)), i)

            Else
                Console.WriteLine("Invalid column: {0}", r.GetName(i))

            End If

        Next

    End Sub

    Public Sub write(ByVal r As SqlDataReader)
        With sw
            For i As Integer = 0 To Count - 1
                If map.Keys.Contains(i) Then
                    sw.Write(String.Format("{0}{1}{0}", Chr(34), r(map(i))))
                End If
                sw.Write(",")
            Next
            sw.Write(vbCrLf)

        End With

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                sw.Close()
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
