import { connect } from 'react-redux';
import { changeQuantity, cancelBasket, checkOutBasket } from '../actions';
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
        checkOutBasket: (basketId, address) => {
            dispatch(checkOutBasket(basketId, address));
        },
        cancelBasket: (basketId) => {
            dispatch(cancelBasket(basketId));
        }

    };
}

const BasketContainer = connect(mapStateToProps, mapDispatchToProps)(Basket);

export default BasketContainer;