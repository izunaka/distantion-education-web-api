from textPreprocessingUtils import processText, correlativeProcessTexts, postprocessTexts, getInputParams
import sys, json

class Object:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__,
            sort_keys=True, indent=4, ensure_ascii=False)

saved = {}

def lev(tokens1, tokens2, i, j):
    if (f'{i}-{j}' in saved):
        return saved[f'{i}-{j}']
    res = None
    if (i == 0 or j == 0):
        res = max(i, j)
    elif (tokens1[i] == tokens2[j]):
        res = 0 + min(lev(tokens1, tokens2, i-1, j-1), lev(tokens1, tokens2, i-1, j), lev(tokens1, tokens2, i, j-1))
    else:
        res = 1 + min(lev(tokens1, tokens2, i-1, j-1), lev(tokens1, tokens2, i-1, j), lev(tokens1, tokens2, i, j-1))
    saved[f'{i}-{j}'] = res
    return res

def mapping(text1, text2, params):
    tokens1 = None; tokens2 = None; fineCoeff = 1
    if (params.synonyms):
        tokens1, tokens2, fine, replacements = correlativeProcessTexts(text1, text2, params.maxfine)
        fineCoeff = 1 - fine
    else:
        tokens1 = processText(text1)
        tokens2 = processText(text2)
    
    tokensList1 = list({k: v for k, v in sorted(tokens1.items(), key=lambda item: item[1])})
    tokensList2 = list({k: v for k, v in sorted(tokens2.items(), key=lambda item: item[1])})
    tokensList1.append(''); tokensList2.append('')
    tokensList1.reverse(); tokensList2.reverse()

    l = lev(tokensList1, tokensList2, len(tokensList1) - 1, len(tokensList2) - 1)
    
    result = Object()
    result.similarity = 1 - (l / max(len(tokensList1) - 1, len(tokensList2) - 1))
    result.generalTerminsInFirstText, \
    result.extraTerminsInFirstText, \
    result.generalTerminsInSecondText, \
    result.extraTerminsInSecondText = postprocessTexts(tokens1, tokens2, text1, text2, replacements)

    return result.toJSON()

args = getInputParams(sys.argv)
print(mapping(args.text1, args.text2, args), end='')