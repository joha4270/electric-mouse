namespace electric_mouse.Models
{
    public class ModalContentViewModel
    {
        public string ViewName { get;}
        public object Model { get; }

        public ModalContentViewModel(string viewName, object model)
        {
            ViewName = viewName;
            Model = model;
        }
    }
}
