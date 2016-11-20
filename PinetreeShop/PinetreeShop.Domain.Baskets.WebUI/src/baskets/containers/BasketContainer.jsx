import { connect } from 'react-redux';
import { changeQuantity, cancel, checkout } from '../actions';
import Basket from '../components/Basket';

const mapStateToProps = (state) => {
    return {
        basket: state.basket
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        changeQuantity: (productId, quantity) => {
            dispatch(changeQuantity(productId, quantity));
        },
        checkout: (basketId, address) => {
            dispatch(checkout(basketId, address));
        },
        cancel: (basketId) => {
            dispatch(cancel(basketId));
        }

    };
}

const BasketContainer = connect(mapStateToProps, mapDispatchToProps)(Basket);

export default BasketContainer;