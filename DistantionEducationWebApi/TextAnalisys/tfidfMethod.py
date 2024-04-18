
from textPreprocessingUtils import processText, correlativeProcessTexts, getVectorsCos, postprocessTexts, getInputParams
from functools import reduce
from math import log2
import sys, json

class Object:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__,
            sort_keys=True, indent=4, ensure_ascii=False)

def mapping(text1, text2, params):
    tokens1 = None; tokens2 = None; fineCoeff = 1
    replacements = {}
    if (params.synonyms):
        tokens1, tokens2, fine, replacements = correlativeProcessTexts(text1, text2, params.maxfine)
        fineCoeff = 1 - fine
    else:
        tokens1 = processText(text1)
        tokens2 = processText(text2)
    
    text1Length = reduce(lambda acc, key: tokens1[key] + acc, tokens1, 0)
    text2Length = reduce(lambda acc, key: tokens2[key] + acc, tokens2, 0)

    allTokens = list(set(tokens1.keys()).union(set(tokens2.keys())))
    text1Vector = []; text2Vector = []

    for token in allTokens:
        tf1 = (tokens1[token] if token in tokens1.keys() else 0) / text1Length
        tf2 = (tokens2[token] if token in tokens2.keys() else 0) / text2Length
        text1Vector.append(tf1)
        text2Vector.append(tf2)

    result = Object()
    result.similarity = getVectorsCos(text1Vector, text2Vector) * fineCoeff
    result.generalTerminsInFirstText, \
    result.extraTerminsInFirstText, \
    result.generalTerminsInSecondText, \
    result.extraTerminsInSecondText = postprocessTexts(tokens1, tokens2, text1, text2, replacements)

    return result.toJSON()

args = getInputParams(sys.argv)
print(mapping(args.text1, args.text2, args), end='')


