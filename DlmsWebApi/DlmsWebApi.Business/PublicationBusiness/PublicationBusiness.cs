using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Repository.PublicationRepository;
using DlmsWebApi.Shared.PublicationData;

namespace DlmsWebApi.Business.PublicationBusiness
{
    public class PublicationBusiness : IPublicationBusiness
    {
        private readonly IPublicationRepository _publicationRepository;

        public PublicationBusiness(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task<bool> Add(PublicationDetails publication)
        {
            var publicationEntity = new Publication
            {
                
                PublicationName = publication.PublicationName,
                PublicationAddress = publication.PublicationAddress,
                PublicationEmail = publication.PublicationEmail,
                ContactPersonName = publication.ContactPersonName,
                ContactPhone = publication.ContactPhone,
                PublicationWebsite = publication.PublicationWebsite,
                Status = publication.Status,
                CreatedBy = publication.User,
                CreatedDate = DateTime.Now
            };
            return await _publicationRepository.Add(publicationEntity);
        }

        public async Task<bool> Edit(PublicationDetails publication)
        {
            return await _publicationRepository.Edit(publication);
        }

        public async Task<PublicationDetails> GetDetails(int id)
        {
            var publicationData = await _publicationRepository.GetDetails(id);
            var publicationDetails = new PublicationDetails
            {
                PublicationId = publicationData.PublicationId,
                PublicationName = publicationData.PublicationName,
                PublicationAddress = publicationData.PublicationAddress,
                PublicationEmail = publicationData.PublicationEmail,
                ContactPersonName = publicationData.ContactPersonName,
                ContactPhone = publicationData.ContactPhone,
                PublicationWebsite = publicationData.PublicationWebsite,
                Status = publicationData.Status
            };
            return publicationDetails;
        }

        public async Task<List<PublicationDetails>> GetList()
        {
            List<PublicationDetails> publicationList = new List<PublicationDetails>();
            var publications = await _publicationRepository.GetList();
            foreach (var publication in publications)
            {
                publicationList.Add(new PublicationDetails
                {
                    PublicationId = publication.PublicationId,
                    PublicationIdString = EncryptionHelper.Encrypt(publication.PublicationId.ToString()),

                    PublicationName = publication.PublicationName,
                    PublicationAddress = publication.PublicationAddress,
                    PublicationEmail = publication.PublicationEmail,
                    ContactPersonName = publication.ContactPersonName,
                    ContactPhone = publication.ContactPhone,
                    PublicationWebsite = publication.PublicationWebsite,
                    Status = string.IsNullOrEmpty(publication.Status) ? "A" : publication.Status,
                });
            }
            return publicationList;
        }

        public async Task<bool> UpdateStatus(int publicationId, string user)
        {
            var result = await _publicationRepository.UpdateStatus(publicationId, user);
            return result;
        }
    }
}
