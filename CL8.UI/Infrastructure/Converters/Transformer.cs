using CL8.BLL.Infrastructure;
using CL8.UI.Models;

namespace CL8.UI.Infrastructure.Converters
{
    public static class Transformer
    {
        public static ChatViewModel ToModel(ChatDto dto) => new()
        { 
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description            
        };

        public static List<ChatViewModel> ToModel(IEnumerable<ChatDto> dtos) => Enumerable.Select(dtos, c => ToModel(c)).ToList();
    }
}