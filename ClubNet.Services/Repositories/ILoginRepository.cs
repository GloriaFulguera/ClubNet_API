namespace ClubNet.Services.Repositories
{
    public interface ILoginRepository
    {
        public Task<LoginResultDTO> Login(LoginDTO login);
        public Task<bool> Register(Login register);
    }
}
