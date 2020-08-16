namespace HsH2BrainEditor.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }

        public ELoginAction Action { get; set; }
    }

    public enum ELoginAction
    {
        Login,
        Registration,
        Error
    }
}
