from textPreprocessingUtils import processText, correlativeProcessTexts, getVectorsCos, postprocessTexts, getInputParams
from transformers import AutoTokenizer, AutoModel
import torch, json, sys

tokenizer = AutoTokenizer.from_pretrained('sentence-transformers/bert-base-nli-mean-tokens')
model = AutoModel.from_pretrained('sentence-transformers/bert-base-nli-mean-tokens')

class Object:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__,
            sort_keys=True, indent=4, ensure_ascii=False)

def mapping(text1, text2, params):
    tokens = tokenizer([text1, text2], max_length=128, truncation=True, padding='max_length', return_tensors='pt')
    outputs = model(**tokens)
    embeddings = outputs.last_hidden_state
    mask = tokens['attention_mask'].unsqueeze(-1).expand(embeddings.size()).float()
    masked_embeddings = embeddings * mask
    summed = torch.sum(masked_embeddings, 1)
    counted = torch.clamp(mask.sum(1), min=1e-9)
    mean_pooled = summed / counted
    score = getVectorsCos(mean_pooled[0], mean_pooled[1]).item()

    tokens1 = None; tokens2 = None
    replacements = {}
    if (params.synonyms):
        tokens1, tokens2, fine, replacements = correlativeProcessTexts(text1, text2, params.maxfine)
    else:
        tokens1 = processText(text1)
        tokens2 = processText(text2)

    result = Object()
    result.similarity = (score - 0.5) * 2
    result.generalTerminsInFirstText, \
    result.extraTerminsInFirstText, \
    result.generalTerminsInSecondText, \
    result.extraTerminsInSecondText = postprocessTexts(tokens1, tokens2, text1, text2, replacements)
    return result.toJSON()

args = getInputParams(sys.argv)
print(mapping(args.text1, args.text2, args), end='')