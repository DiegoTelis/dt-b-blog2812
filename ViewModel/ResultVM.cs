namespace Blog.ViewModel
{
    public class ResultVM<T>
    {
        public ResultVM(T data, List<string> erros)
        {
            Data = data;
            Erros = erros;
        }

        public ResultVM(T data)
        {
            Data = data;
        }

        public ResultVM(List<string> erros)
        {
            Erros = erros;
        }

        public ResultVM( string error)
        {
            Erros.Add(error);
        }
        //criar crud de pessoa  

        public T Data { get; private set; }
        public List<String> Erros { get; private set; } = new();

    }
}
