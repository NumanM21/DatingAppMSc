using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        
        Task<IEnumerable<ApprovePhotoDto>> PhotosUnapprovedDtoGetter();
        Task<Photo> PhotoByIdGetter(int id);
        void PhotoRemove(Photo photo); 
    }
}