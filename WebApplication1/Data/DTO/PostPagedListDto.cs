namespace WebApplication1.Data.DTO
{
    public class PostPagedListDto
    {
        public List<PostDto> posts { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
