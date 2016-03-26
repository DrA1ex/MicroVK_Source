

Namespace Api
    Class Store
        Shared Async Function GetProducts() As Task(Of types.Product())
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.Product())("store.getProducts@items",
                                                                {{"type", "stickers"}, {"filters", "purchased"},
                                                                 {"extended", "1"}})
        End Function
    End Class
End NameSpace