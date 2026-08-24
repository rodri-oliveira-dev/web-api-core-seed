using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiCoreSeed.SampleRestaurant.Infrastructure.Context;
using WebApiCoreSeed.SampleRestaurant.Models;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public sealed class DevelopmentSeedSampleRestaurantSeeder
    {
        private readonly SampleRestaurantDbContext _context;

        public DevelopmentSeedSampleRestaurantSeeder(SampleRestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<int> SeedAsync(CancellationToken cancellationToken = default)
        {
            var changes = 0;
            changes += await UpsertAtendenteAsync(cancellationToken);

            foreach (var mesa in DevelopmentSeedDefinition.Mesas)
            {
                changes += await UpsertMesaAsync(mesa, cancellationToken);
            }

            foreach (var prato in DevelopmentSeedDefinition.Pratos)
            {
                changes += await UpsertPratoAsync(prato, cancellationToken);
            }

            changes += await UpsertPedidoAsync(cancellationToken);

            foreach (var pedidoPrato in DevelopmentSeedDefinition.PedidoPratos)
            {
                changes += await UpsertPedidoPratoAsync(pedidoPrato, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return changes;
        }

        private async Task<int> UpsertAtendenteAsync(CancellationToken cancellationToken)
        {
            var seed = DevelopmentSeedDefinition.Atendente;
            var entity = await _context.Atendentes.FindAsync([seed.Id], cancellationToken);

            if (entity is null)
            {
                _context.Atendentes.Add(new Atendente
                {
                    Id = seed.Id,
                    Nome = seed.Nome,
                    TipoAtendente = seed.TipoAtendente
                });
                return 1;
            }

            var changed = false;
            changed |= SetIfDifferent(entity.Nome, seed.Nome, value => entity.Nome = value);
            changed |= SetIfDifferent(entity.TipoAtendente, seed.TipoAtendente, value => entity.TipoAtendente = value);

            if (changed)
            {
                _context.Atendentes.Update(entity);
            }

            return changed ? 1 : 0;
        }

        private async Task<int> UpsertMesaAsync(DevelopmentSeedMesa seed, CancellationToken cancellationToken)
        {
            var entity = await _context.Mesas.FindAsync([seed.Id], cancellationToken);

            if (entity is null)
            {
                _context.Mesas.Add(new Mesa
                {
                    Id = seed.Id,
                    Numero = seed.Numero,
                    Lugares = seed.Lugares,
                    Ativo = seed.Ativo,
                    LocalizacaoMesa = seed.LocalizacaoMesa
                });
                return 1;
            }

            var changed = false;
            changed |= SetIfDifferent(entity.Numero, seed.Numero, value => entity.Numero = value);
            changed |= SetIfDifferent(entity.Lugares, seed.Lugares, value => entity.Lugares = value);
            changed |= SetIfDifferent(entity.Ativo, seed.Ativo, value => entity.Ativo = value);
            changed |= SetIfDifferent(entity.LocalizacaoMesa, seed.LocalizacaoMesa, value => entity.LocalizacaoMesa = value);

            if (changed)
            {
                _context.Mesas.Update(entity);
            }

            return changed ? 1 : 0;
        }

        private async Task<int> UpsertPratoAsync(DevelopmentSeedPrato seed, CancellationToken cancellationToken)
        {
            var entity = await _context.Pratos.FindAsync([seed.Id], cancellationToken);

            if (entity is null)
            {
                _context.Pratos.Add(new Prato
                {
                    Id = seed.Id,
                    Titulo = seed.Titulo,
                    Descricao = seed.Descricao,
                    Foto = seed.Foto,
                    Preco = seed.Preco,
                    Ativo = seed.Ativo,
                    TipoPrato = seed.TipoPrato
                });
                return 1;
            }

            var changed = false;
            changed |= SetIfDifferent(entity.Titulo, seed.Titulo, value => entity.Titulo = value);
            changed |= SetIfDifferent(entity.Descricao, seed.Descricao, value => entity.Descricao = value);
            changed |= SetIfDifferent(entity.Foto, seed.Foto, value => entity.Foto = value);
            changed |= SetIfDifferent(entity.Preco, seed.Preco, value => entity.Preco = value);
            changed |= SetIfDifferent(entity.Ativo, seed.Ativo, value => entity.Ativo = value);
            changed |= SetIfDifferent(entity.TipoPrato, seed.TipoPrato, value => entity.TipoPrato = value);

            if (changed)
            {
                _context.Pratos.Update(entity);
            }

            return changed ? 1 : 0;
        }

        private async Task<int> UpsertPedidoAsync(CancellationToken cancellationToken)
        {
            var seed = DevelopmentSeedDefinition.Pedido;
            var entity = await _context.Pedidos.FindAsync([seed.Id], cancellationToken);

            if (entity is null)
            {
                _context.Pedidos.Add(new Pedido
                {
                    Id = seed.Id,
                    AtendenteId = seed.AtendenteId,
                    MesaId = seed.MesaId,
                    Numero = seed.Numero,
                    DataHoraCadastro = seed.DataHoraCadastro
                });
                return 1;
            }

            var changed = false;
            changed |= SetIfDifferent(entity.AtendenteId, seed.AtendenteId, value => entity.AtendenteId = value);
            changed |= SetIfDifferent(entity.MesaId, seed.MesaId, value => entity.MesaId = value);
            changed |= SetIfDifferent(entity.Numero, seed.Numero, value => entity.Numero = value);
            changed |= SetIfDifferent(entity.DataHoraCadastro, seed.DataHoraCadastro, value => entity.DataHoraCadastro = value);

            if (changed)
            {
                _context.Pedidos.Update(entity);
            }

            return changed ? 1 : 0;
        }

        private async Task<int> UpsertPedidoPratoAsync(DevelopmentSeedPedidoPrato seed, CancellationToken cancellationToken)
        {
            var entity = await _context.PedidoPrato.FindAsync([seed.Id], cancellationToken);

            if (entity is null)
            {
                _context.PedidoPrato.Add(new PedidoPrato
                {
                    Id = seed.Id,
                    PedidoId = seed.PedidoId,
                    PratoId = seed.PratoId,
                    StatusProducao = seed.StatusProducao,
                    Observacao = seed.Observacao
                });
                return 1;
            }

            var changed = false;
            changed |= SetIfDifferent(entity.PedidoId, seed.PedidoId, value => entity.PedidoId = value);
            changed |= SetIfDifferent(entity.PratoId, seed.PratoId, value => entity.PratoId = value);
            changed |= SetIfDifferent(entity.StatusProducao, seed.StatusProducao, value => entity.StatusProducao = value);
            changed |= SetIfDifferent(entity.Observacao, seed.Observacao, value => entity.Observacao = value);

            if (changed)
            {
                _context.PedidoPrato.Update(entity);
            }

            return changed ? 1 : 0;
        }

        private static bool SetIfDifferent<T>(T current, T expected, System.Action<T> setValue)
        {
            if (Equals(current, expected))
            {
                return false;
            }

            setValue(expected);
            return true;
        }
    }
}
