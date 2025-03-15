namespace WebApplication1.Data.DTO.Post
{
    public class PostPagedListDto
    {
        public List<PostDto> posts { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
