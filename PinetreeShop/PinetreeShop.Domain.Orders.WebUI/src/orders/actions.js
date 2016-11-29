import fetch from 'isomorphic-fetch';
import config from '../config';

const createRequest = (method, body) => ({
    method,
    body,
    headers: {
        'Content-Type': 'application/json'
    }
})

const createPostRequest = (body) => createRequest('POST', JSON.stringify(body));
const createGetRequest = () => createRequest('GET');

export const actionTypes = {
    ADD_OR_UPDATE_ORDER: 'ADD_OR_UPDATE_ORDER',
    CANCEL_ORDER: 'CANCEL_ORDER',
    PREPARE_ORDER_FOR_SHIPPING: 'PREPARE_ORDER_FOR_SHIPPING',
    SHIP_ORDER: 'SHIP_ORDER',
    DELIVER_ORDER: 'DELIVER_ORDER'
}

export const getOrders = () => {
    return dispatch => {
        const url = config.API_URL + '/list';

        fetch(url, createGetRequest(url))
            .then(response => response.json())
            .then(orders => {
                orders.forEach(order => dispatch({
                    type: actionTypes.ADD_OR_UPDATE_ORDER,
                    order
                }));
            });
    }
}

const updateOrder = (orderId, urlSuffix) => {
    return dispatch => {
        const url = config.API_URL + '/' + urlSuffix;

        fetch(url, createPostRequest({ orderId }))
            .then(response => response.json())
            .then(order => dispatch({
                type: actionTypes.ADD_OR_UPDATE_ORDER,
                order
            }));
    };
}

export const cancelOrder = (orderId) => {
    return updateOrder(orderId, 'cancel')
}
export const prepareOrderForShipping = (orderId) => {
    return updateOrder(orderId, 'prepareForShipping')
}
export const shipOrder = (orderId) => {
    return updateOrder(orderId, 'ship')
}
export const deliverOrder = (orderId) => {
    return updateOrder(orderId, 'deliver')
}