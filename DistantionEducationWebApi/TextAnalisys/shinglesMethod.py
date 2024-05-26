from textPreprocessingUtils import processText, correlativeProcessTexts, postprocessTexts, getInputParams
import sys, json

class Object:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__,
            sort_keys=True, indent=4, ensure_ascii=False)

def mapping(text1, text2, params):
    tokens1 = None; tokens2 = None; fineCoeff = 1
    if (params.synonyms):
        tokens1, tokens2, fine, replacements = correlativeProcessTexts(text1, text2, params.maxfine)
        fineCoeff = 1 - fine
    else:
        tokens1 = processText(text1)
        tokens2 = processText(text2)
    
    tokensSet1 = set(tokens1.keys())
    tokensSet2 = set(tokens2.keys())

    similarity = None

    if (params.frequency):
        intersectionCount = 0
        unionCount = 0
        for item in tokensSet1.intersection(tokensSet2):
            intersectionCount += min(tokens1[item], tokens2[item])
        for item in tokensSet1.union(tokensSet2):
            unionCount += max(tokens1[item] if item in tokens1.keys() else 0, tokens2[item]if item in tokens2.keys() else 0)
        similarity =  intersectionCount / unionCount * fineCoeff
    else:
        similarity = len(tokensSet1.intersection(tokensSet2)) / len(tokensSet1.union(tokensSet2)) * fineCoeff
    
    result = Object()
    result.similarity = similarity
    result.generalTerminsInFirstText, \
    result.extraTerminsInFirstText, \
    result.generalTerminsInSecondText, \
    result.extraTerminsInSecondText = postprocessTexts(tokens1, tokens2, text1, text2, replacements)

    return result.toJSON()

args = getInputParams(sys.argv)
print(mapping(args.text1, args.text2, args), end='')