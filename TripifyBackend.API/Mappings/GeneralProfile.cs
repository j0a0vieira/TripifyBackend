using AutoMapper;
using Microsoft.Extensions.DependencyInjection; 
using TripifyBackend.DOMAIN.Models;
using System;
using System.Collections.Generic;
using TripifyBackend.INFRA.Entities;

namespace TripifyBackend.API.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile() 
        {
            CreateMap<DeserializePlace.Categories, CategoriesDomain>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));

            CreateMap<DeserializePlace, PlaceDomain>()
                .ConstructUsing((src, context) =>
                {
                    var categoriesDomain = context.Mapper.Map<List<CategoriesDomain>>(src.categories);

                    // Criando o PlaceDomain
                    var placeDomain = new PlaceDomain
                    {
                        Id = Guid.NewGuid(),
                        Name = src.name,
                        Latitude = src.geocodes.main.latitude,
                        Longitude = src.geocodes.main.longitude,
                        FormattedAddress = src.location.formatted_address,
                        Locality = src.location.locality,
                        Postcode = src.location.postcode,
                        Country = src.location.country,
                        Timezone = src.timezone,
                        Region = src.location.region,
                        Categories = categoriesDomain
                    };

                    return placeDomain;
                });

            CreateMap<PlaceDomain, PlaceDB>();
            CreateMap<CategoriesDomain, CategoriesDB>();
        }
    }
}