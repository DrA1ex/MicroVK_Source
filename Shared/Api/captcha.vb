Imports Newtonsoft.Json.Linq

Namespace Api
    Class Captcha
        Public Async Sub Force()
            Await MicroVkApi.GetResponse (Of JToken)("captcha.force")
        End Sub
    End Class
End NameSpace