using AutoMapper;
using Models.Entities;
using API.DTOs.Category;
using API.DTOs.Product;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryReadDTO>();
        CreateMap<CategoryCreateDTO, Category>();
        CreateMap<CategoryUpdateDTO, Category>().ReverseMap();

        //// Product mappings
        CreateMap<Product, ProductReadDTO>();
        CreateMap<ProductCreateDTO, Product>();
        CreateMap<ProductUpdateDTO, Product>().ReverseMap();
    }
}
