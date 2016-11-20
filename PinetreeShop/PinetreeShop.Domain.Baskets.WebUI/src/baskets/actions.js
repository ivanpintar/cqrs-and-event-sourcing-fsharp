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
const createGetRequest = (body) => createRequest('GET');

export const actionTypes = {
    UPDATE_BASKET: 'UPDATE_BASKET',
    CHANGE_QUANTITY: 'CHANGE_QUANTITY',
    CHECKOUT: 'CHECKOUT',
    CANCEL: 'CANCEL'
}

export const dispatchBasketUpdate = (dispatch, basket) => {
    localStorage.setItem('basketId', basket.id);

    dispatch({
        type: actionTypes.UPDATE_BASKET,
        basket
    });
}

export const changeQuantity = (productId, quantity) => {
    return (dispatch, getState) => {
        const state = getState();
        const order = state.basket.orderLines.find(p => p.productId === productId);
        const product = state.products.find(p => p.id === productId);

        let url = '';
        let postModel = {};

        if(quantity > order.quantity){
            url = config.BASKET_API_URL + '/addItem';
            postModel = { 
                basketId: state.basket.id,
                productId,
                name: product.name,
                price: product.price,
                quantity: quantity - order.quantity 
            };
        } else {
            url = config.BASKET_API_URL + '/removeItem';
            postModel = {
                basketId: state.basket.id,
                productId,
                quantity: order.quantity - quantity
            };
        }
                
        fetch(url, createPostRequest(postModel))
            .then(response => response.json())        
            .then(basket => dispatchBasketUpdate(dispatch, basket));
    };
}

export const getBasket = (basketId) => {
    return dispatch => {
        const url = config.BASKET_API_URL + '/' + basketId;

        fetch(url, createGetRequest(url))
            .then(response => response.json())
            .then(basket => {
                dispatch({
                    type: actionTypes.UPDATE_BASKET,
                    basket
                });
            });
    }
}