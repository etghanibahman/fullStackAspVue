
<template>
    <div id="app" class="newapp">
        <div class="row">
            <div class="col-md-7 col-12" id="divSearch">

                <h3>forecast for 5 days</h3>
                <label class="radio-inline" style="padding:1% 2%"><input type="radio" name="optradio" checked v-on:click="isHidden = true">Search by Name</label>
                <label class="radio-inline" style="padding:1% 2%"><input type="radio" name="optradio" v-on:click="isHidden = !isHidden">Search by Postcode</label>
                <div id="divPostcode" v-if="!isHidden">
                    <h2>ZIP Code</h2>
                    <input class="form-control" id="zipCode" maxlength="20" name="zipCode" type="number" />
                    <hr />
                    <button class="btn btn-primary" id="btnZipCode" v-on:click="callApiForecast">Forecast</button>
                </div>
                <div id="divCityName" v-if="isHidden">
                    <b-dropdown id="ddCommodity"
                                name="ddCommodity"
                                v-model="ddTestVm.ddTestSelectedOption"
                                text="Select a city"
                                variant="primary"
                                class="m-md-2">
                        <b-dropdown-item disabled value="0">Select a city</b-dropdown-item>
                        <b-dropdown-item v-for="option in ddTestVm.options"
                                         :key="option.value"
                                         :value="option.value"
                                         @click="ddTestVm.ddTestSelectedOption = option.value">
                            {{option.text}}
                        </b-dropdown-item>
                    </b-dropdown>
                    <br />
                    <span class="label label-default">Selected item: <strong id="cityName">{{ddTestVm.ddTestSelectedOption}}</strong></span>
                    <hr />
                    <button class="btn btn-primary" id="btnCityName" v-on:click="callApiForecast">Forecast</button>
                </div>

                <!--<ForecastData :searchResult="searchResultP"></ForecastData>-->
                <div v-if="!loadingFlag && !errorHappened">
                    <div id="cityNameDiv" name="cityNameDiv" style="padding: 2% 0;" v-if="searchResultParent">
                        <span class="label label-default">forecasts for <strong>{{queryType}}</strong></span>
                    </div>
                    <b-table :items="searchResultParent">
                        <template v-slot:cell(name)="data">

                        </template>
                    </b-table>
                </div>
                <div class="form-group" id="errorDiv" v-if="errorHappened">
                    <span class="label label-default">{{errorMessage}}</span>
                </div>
                <div class="form-group" id="loadingDiv" v-if="loadingFlag">
                    <span class="label label-default">Loading...</span>
                </div>
            </div>
            <div class="col-md-5 col-12" id="divHistory">
                <h3>History</h3>
                <hr />
                <div id="historyExDiv" name="historyExDiv" style="padding: 2% 0;" v-if="historyResultData">
                    <span class="label label-default">Here you can see the average for coming five days from the data that you have searched so far.</span>
                </div>
                <div>
                    <b-table :items="historyResultData">
                        <template v-slot:cell(name)="data">

                        </template>
                    </b-table>
                </div>
            </div>

        </div>

    </div>
</template>

<script>
    import axios from 'axios'
    export default {
        name: 'app',
        props: {
            searchResultParent: [],
            historyResultData: [],
            errorHappened: Boolean,
            errorMessage: String,
            queryType: String,
            loadingFlag: Boolean,
        },
        mounted: function () {
            let list = [];
            let res = [];
            this.errorHappened = false;
            this.loadingFlag = false;
            axios.get('/api/values/')
                .then((response) => {
                    res = response.data;
                    res.forEach((value, index) => {
                        list.push({
                            "value": value,
                            "text": value
                        });
                    });
                    this.ddTestVm.options = list;
                });
        },
        data() {
            return {
                items: [
                ],
                isHidden: true,
                someOtherProperty: null,
                ddTestVm: {
                    originalValue: [],
                    ddTestSelectedOption: "Munich",
                    disabled: false,
                    readonly: false,
                    visible: true,
                    color: "",
                    options: []
                }
            }
        },
        methods: {
            callApiForecast: function (event) {
                this.loadingFlag = true;
                this.errorHappened = false;
                let historyResultP = [];
                let searchResultP = [];
                let tempQueryType = '';
                if (event) {
                    let queryPhrase = "";
                    if (event.currentTarget.id == "btnCityName") {
                        queryPhrase = document.getElementById("cityName").innerText;
                    } else {
                        queryPhrase = document.getElementById("zipCode").value;
                    }
                    axios.get('/api/values/' + queryPhrase)
                        .then(response => {
                            this.loadingFlag = false;
                            if (response.data) {
                                if (response.data[0].queryType) {
                                    tempQueryType = response.data[0].queryType;
                                    searchResultP = response.data.map(function (obj) {
                                        return {
                                            Date: obj.date,
                                            Temperature: obj.temperature,
                                            Humidity: obj.humidity
                                        }
                                    });
                                    this.queryType = tempQueryType;
                                    this.searchResultParent = searchResultP;
                                } else {
                                    this.loadingFlag = false;
                                    this.errorHappened = true;
                                    this.errorMessage = "There was no result for your query.";
                                }
                            } else {
                                this.errorHappened = true;
                                if (response.statusText) {
                                    this.errorMessage = response.statusText;
                                } else {
                                    this.errorMessage = "There was no result for your query.";
                                }
                            }

                        }).catch(error => {

                            this.loadingFlag = false;
                            this.errorHappened = true;
                            if (Object.assign({}, error).response.data) {
                                this.errorMessage = Object.assign({}, error).response.data;
                            } else {
                                this.errorMessage = "There was no result for your query.";
                            }

                        }).then(() =>
                            // History
                            axios.get('/api/values/history')
                                .then(response => {
                                    historyResultP = response.data;
                                    this.historyResultData = historyResultP;
                                }).catch(error => {
                                    console.log(error.data);
                                })
                        )
                }
            }
        }
    }
</script>
<style>
    .newapp {
        max-width: 90%;
        padding:4% 6%;
        margin: 2% auto;
        margin-bottom: 0px !important;
        background-color:aliceblue;
        min-height: 100%;
        border-radius:15px; 
    }

    #divSearch > * {
        max-width: 70%;
    }
</style>
