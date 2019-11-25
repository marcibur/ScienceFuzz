var axios = require('axios');
var tsne = require('tsne-js');

module.exports = async function (context, req, scientists) {
    const uri = process.env["DOMAINS_CONTRIBUTIONS_URI"];

    let { data } = await axios.get(uri);
    let contributionSets = data;

    var tsneInput = [];
    var names = [];
    contributionSets.forEach((set) => {
        names.push(set.scientist);
        var input = [];
        set.contributions.forEach((contribution) => {
            input.push(contribution.value);
        });
        tsneInput.push(input);
    });

    let model = new tsne({
        dim: 2,
        perplexity: 5,
        earlyExaggeration: 1.0,
        learningRate: 1000.0,
        nIter: 1000000,
        metric: 'euclidean'
    });

    model.init({
        data: tsneInput,
        type: 'dense'
    });

    model.run();
    let tsneOutput = model.getOutput();

    let output = [];
    for (let i = 0; i < scientists.length; i++) {
        let result = {
            scientist: names[i],
            point: {
                x: tsneOutput[i][0],
                y: tsneOutput[i][1]
            }
        };
        output.push(result);
    }

    return output;
};