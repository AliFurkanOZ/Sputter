using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SputterNewApp.Infrastructure.Serial
{
    public sealed class MultiStm2Manager : IDisposable
    {
        private readonly List<Stm2Client> _clients = new();

        public IReadOnlyList<Stm2Client> Clients => _clients;

        public MultiStm2Manager(IEnumerable<string> portNames)
        {
            foreach (var p in portNames.Where(x => !string.IsNullOrWhiteSpace(x)))
                _clients.Add(new Stm2Client(p.Trim()));
        }

        public void OpenAll()
        {
            foreach (var c in _clients) c.Open();
        }

        public void CloseAll()
        {
            foreach (var c in _clients) c.Close();
        }

        public void Dispose()
        {
            foreach (var c in _clients) c.Dispose();
            _clients.Clear();
        }

        /// <summary>
        /// Tüm cihazlara aynı anda ping atar: her biri "@"
        /// </summary>
        public async Task<Dictionary<string, string>> PingAllAsync(int timeoutMs = 1200, CancellationToken ct = default)
        {
            var tasks = _clients.Select(async c =>
            {
                try
                {
                    var msg = await c.PingAsync(timeoutMs, ct).ConfigureAwait(false);
                    return (c.PortName, ok: true, msg);
                }
                catch (Exception ex)
                {
                    return (c.PortName, ok: false, msg: ex.Message);
                }
            });

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results.ToDictionary(x => x.PortName, x => x.ok ? x.msg : $"ERR: {x.msg}");
        }
    }
}
