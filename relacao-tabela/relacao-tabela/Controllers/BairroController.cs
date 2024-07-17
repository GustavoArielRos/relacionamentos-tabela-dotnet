using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using relacao_tabela.Data;
using relacao_tabela.Dtos;
using relacao_tabela.Models;

namespace relacao_tabela.Controllers
{
    [Route("api/[controller]")] //definindo a roda base para o controlador ( api/bairro )
    [ApiController] //esse controlador responde solicitações API
    public class BairroController : Controller
    {
        private readonly AppDbContext _context;//variável que armazena o contexto do banco de dados

        public BairroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        //método assíncrono que recebe um DTO para criar uma casa
        public async Task<IActionResult> criarCasa(CasaCriacaoDto casa)
        {
            //criando uma casa
            var novaCasa = new CasaModel() 
            {
                Descricao = casa.Descricao
            };

            //criando um endereco
            var endereco = new EnderecoModel() 
            {
                Rua = casa.endereco.Rua,
                Numero = casa.endereco.Numero
            };

            //cria uma LISTA de quartos a partir do DTO recebido (casa)
            var quartos = casa.quartos.Select(q => new QuartoModel//para cada quarto criasse um objeto quartomodel  
            { 
                Descricao = q.Descricao, //define a descrição do quarto
                Casa = novaCasa  //associa o quarto à nova casa
            
            }).ToList();//criando a lista

            //cria uma LISTA de moradores a partir do DTO recebido (casa)
            var moradores = casa.moradores.Select(m => new MoradorModel//para cada morador criasse um objeto moradormodel
            { 
                Nome = m.Nome,//define o nome do morador 
                Casas = new List<CasaModel> { novaCasa }//o morador vai ser a associada a um conjuto de casas
                                                        
            }).ToList();//criando a lista


            //colocando as variaveis criadas na variavel casa criada
            novaCasa.Endereco = endereco;
            novaCasa.Quartos = quartos;
            novaCasa.Moradores = moradores;

            //adicionando a casa criada no banco de dados
            _context.Casas.Add(novaCasa);
            //salvando as alterações
            await _context.SaveChangesAsync();

            //faz a consulta no banco de dados retorna todas as casas com todos o includes informados:
            //await _context.Casas --> vai pegar as informações da tabela Casas
            //.Include(e => e.Endereco) --> incluir a entidade endereco da casa
            //.ToListAsync() --> faz a consulta no banco de dados e retorna uma lista com os resultados
            return Ok(await _context.Casas.Include(e => e.Endereco).Include(q => q.Quartos).Include(m => m.Moradores).ToListAsync());
        }


    }
}
