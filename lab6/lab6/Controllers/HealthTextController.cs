using Azure.AI.TextAnalytics;
using lab6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab6.Controllers
{
    public class HealthTextController : Controller
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public HealthTextController(TextAnalyticsClient textAnalyticsClient)
        {
            _textAnalyticsClient = textAnalyticsClient;
        }

        public IActionResult Index()
        {
            return View(new HealthTextModel());
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(HealthTextModel model)
        {
            if (string.IsNullOrWhiteSpace(model.InputText))
            {
                ModelState.AddModelError("InputText", "Please enter some text.");
                return View("Index", model);
            }

            await ProcessText(model);
            return View("Index", model);
        } 

        private async Task ProcessText(HealthTextModel model)
        {
            string processedText = model.InputText;
            var piiEntities = await ExtractPiiEntities(model.InputText);
            model.PIIs = piiEntities;
            processedText = ReplacePiiEntities(processedText, piiEntities);

            var (medicalEntities, medmedicalRelations) = await ExtractMedicalEntities(model.InputText);
            model.MedicalEntities = medicalEntities;
            model.MedicalRelations = medmedicalRelations;
            processedText = HighlightMedicalEntities(processedText, medicalEntities);

            model.OutputText = NormalizeText(processedText);
        }

        private async Task<List<PII>> ExtractPiiEntities(string inputText)
        {
            var piiEntities = new List<PII>();
            var piiResponse = await _textAnalyticsClient.RecognizePiiEntitiesAsync(inputText);

            if (piiResponse.Value != null)
            {
                piiEntities = piiResponse.Value.Select(entity => new PII
                {
                    Text = entity.Text,
                    Category = entity.Category.ToString(),
                    SubCategory = entity.SubCategory?.ToString() ?? "N/A",
                    ConfidenceScore = entity.ConfidenceScore
                }).ToList();
            }

            return piiEntities;
        }

        private string ReplacePiiEntities(string inputText, List<PII> piiEntities)
        {
            string processedText = inputText;

            foreach (var entity in piiEntities)
            {
                string entityWithSymbols = string.Join("%%", entity.Text.ToCharArray());

                string hiddenText = $"<span class='bg-warning badge text-dark' title='{entityWithSymbols}'>[{entity.Category}]</span>";
                processedText = processedText.Replace(entity.Text, hiddenText);
            }
            return processedText;
        }


        private async Task<(List<MedicalEntity>, List<MedicalRelation>)> ExtractMedicalEntities(string inputText)
        {
            var medicalEntities = new List<MedicalEntity>();
            var medicalRelations = new List<MedicalRelation>();

            List<string> batchInput = new List<string> { inputText };
            AnalyzeHealthcareEntitiesOperation healthOperation = await _textAnalyticsClient.StartAnalyzeHealthcareEntitiesAsync(batchInput);
            await healthOperation.WaitForCompletionAsync();

            await foreach (var result in healthOperation.Value)
            {
                foreach (AnalyzeHealthcareEntitiesResult entitiesInDoc in result)
                {
                    if (!entitiesInDoc.HasError)
                    {
                        foreach (var entity in entitiesInDoc.Entities)
                        {
                            medicalEntities.Add(new MedicalEntity
                            {
                                Text = entity.Text,
                                Category = entity.Category.ToString(),
                                ConfidenceScore = entity.ConfidenceScore
                            });
                        }
                        foreach (var relation in entitiesInDoc.EntityRelations)
                        {
                            var medicalRelation = new MedicalRelation
                            {
                                RelationType = relation.RelationType.ToString(),
                                Roles = relation.Roles.Select(role => new MedicalRole
                                {
                                    RoleName = role.Name,
                                    EntityText = role.Entity.Text,
                                    EntityCategory = role.Entity.Category.ToString()
                                }).ToList()
                            };
                            medicalRelations.Add(medicalRelation);
                        }
                    }
                }
            }
            return (medicalEntities, medicalRelations);
        }

        private string HighlightMedicalEntities(string inputText, List<MedicalEntity> medicalEntities)
        {
            string processedText = inputText;
            foreach (var entity in medicalEntities)
            {
                string entityWithSymbols = string.Join("%%", entity.Text.ToCharArray());

                string highlightedText = $"<span class='badge bg-success text-light' title='{entity.Category}'>{entityWithSymbols}</span>";
                processedText = processedText.Replace(entity.Text, highlightedText);
            }
            return processedText;
        }
        private string NormalizeText(string inputText)
        {
            return inputText.Replace("%%", "");
        }
    }
}
