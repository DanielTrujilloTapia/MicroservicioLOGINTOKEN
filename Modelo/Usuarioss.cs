using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservicio.Login.Api.Modelo
{
    public class Usuarioss
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("usuario")]
        public string Usuario { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [BsonElement("usuarioGuid")]
        public string UsuarioGuid { get; set; }

        [BsonElement("preguntaRecuperacion")]
        public string PreguntaRecuperacion { get; set; }

        [BsonElement("respuestaRecuperacion")]
        public string RespuestaRecuperacion { get; set; }

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; }

        [BsonElement("refreshTokenExpiration")]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}