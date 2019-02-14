Public Class importCol
    Private _type As String
    Public Property Type As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
        End Set
    End Property

    Sub New(Name As String, Type As String)
        _Name = Name
        _type = Type

    End Sub

End Class
