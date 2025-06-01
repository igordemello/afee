using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace tcc_in305b.Services
{
    public class ValorantApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ValorantApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = "HDEV-84b47eca-1c4d-443a-8c55-322e9f885e59";  // Substitua pela sua chave de API
        }

        // Exemplo: Obter informações de um jogador
        public async Task<PlayerInfo> GetPuuid(string playerName, string tag)
        {
            var url = $"https://api.henrikdev.xyz/valorant/v1/account/{playerName}/{tag}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(request);

            // Verifica se o status da resposta é 404 (não encontrado)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Retorna null se a conta não for encontrada
                return null;
            }

            // Se a resposta não for bem-sucedida (mas não 404), lance uma exceção
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao buscar o PUUID: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);
            var playerData = jsonResponse.data;

            return new PlayerInfo
            {
                Puuid = playerData.puuid
            };
        }

        public async Task<PlayerInfo> GetElo(string puuid)
        {
            var url = $"https://api.henrikdev.xyz/valorant/v1/by-puuid/mmr-history/na/{puuid}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(request);

            // Verifica se o status da resposta é 404 (não encontrado)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Conta não encontrada
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao buscar o Elo: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);

            // Verifique se 'data' existe e contém itens
            if (jsonResponse?.data != null && jsonResponse.data.Count > 0)
            {
                // Acessa o primeiro item no array 'data'
                var playerData = jsonResponse.data[0];
                return new PlayerInfo
                {
                    Elo = playerData.currenttierpatched.ToString()  // Acessa currenttierpatched corretamente
                };
            }

            // Retorna null se 'data' não estiver presente ou estiver vazio
            return null;
        }
        public async Task<(int kills, int deaths, int assists, double hsPercentage, double winRate, double damage, int firstBloods)> GetPlayerStats(string puuid)
        {
            var url = $"https://api.henrikdev.xyz/valorant/v4/by-puuid/matches/na/pc/{puuid}?mode=competitive&size=10";
            Console.WriteLine($"URL corrigida: {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"PUUID não encontrado ou sem partidas competitivas: {puuid}");
                return (0, 0, 0, 0.0, 0.0, 0, 0);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao buscar as partidas: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);

            if (jsonResponse?.data == null || !jsonResponse.data.HasValues)
            {
                Console.WriteLine($"Nenhum dado de partida encontrado para PUUID: {puuid}");
                return (0, 0, 0, 0.0, 0.0, 0, 0);
            }

            int totalKills = 0;
            int totalDeaths = 0;
            int totalAssists = 0;
            int totalShots = 0;
            int headshots = 0;
            int totalMatches = 0;
            int totalWins = 0;
            int totalDamage = 0;
            int totalRounds = 0;
            int totalFirstBloods = 0;
            foreach (var partida in jsonResponse.data)
            {
                totalMatches++;
                bool isWinner = false;
                var roundKills = new Dictionary<int, (double time, string killerPuuid)>();
                foreach (var kill in partida.kills)
                {
                    int round = kill.round;
                    double killTime = kill.time_in_round_in_ms;
                    string killerPuuid = kill.killer.puuid;

                    if (!roundKills.ContainsKey(round) || killTime < roundKills[round].time)
                    {
                        roundKills[round] = (killTime, killerPuuid);
                    }
                }

                foreach (var round in roundKills)
                {
                    if (round.Value.killerPuuid == puuid)
                    {
                        totalFirstBloods++;
                    }
                }
                foreach (var player in partida.players)
                {
                    if (player.puuid == puuid)
                    {
                        totalKills += (int)player.stats.kills;
                        totalDeaths += (int)player.stats.deaths;
                        totalAssists += (int)player.stats.assists;

                        // Soma os Tiros no Coco, Peito e Perna(kk newba)
                        headshots += (int)player.stats.headshots;
                        totalShots += (int)player.stats.legshots + (int)player.stats.headshots + (int)player.stats.bodyshots;
                        totalDamage += (int)player.stats.damage.dealt;
                        break;
                    }

                }
                totalRounds += (int)partida.rounds.Count;
                foreach (var team in partida.teams)
                {
                    if (team.won == true)
                    {
                        foreach (var player in partida.players)
                        {
                            if (player.puuid == puuid && player.team_id == team.team_id)
                            {
                                isWinner = true;
                                break;
                            }
                        }
                    }
                }

                if (isWinner == true)
                {
                    totalWins++;
                }

            }
            double avgDamagePerRound = totalRounds > 0 ? totalDamage / totalRounds : 0.0;
            double hsPercentage = totalShots > 0 ? (double)headshots / totalShots * 100 : 0.0;
            double winRate = totalMatches > 0 ? (double)totalWins / totalMatches * 100 : 0.0;
            Console.WriteLine($"Total: Kills={totalKills}, Deaths={totalDeaths}, Assists={totalAssists}, HS%={hsPercentage}");
            return (totalKills, totalDeaths, totalAssists, hsPercentage, winRate, avgDamagePerRound, totalFirstBloods);
        }
    }

    public class PlayerInfo
    {
        public string Puuid { get; set; }
        public string Elo {  get; set; }

    }
}
