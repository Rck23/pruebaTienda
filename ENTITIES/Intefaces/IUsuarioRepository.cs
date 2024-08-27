using ENTITIES.Entities;
namespace ENTITIES.Intefaces;

public interface IUsuarioRepository : IGenericRepository<Usuario> { 
    
    Task<Usuario> GetByUsernameAsync(string username);

    Task<Usuario> GetByRefreshTokenAsync(string refreshToken);
}

