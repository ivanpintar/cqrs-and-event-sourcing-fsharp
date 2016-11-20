import React from 'react';
import { Button, Table } from 'react-bootstrap';
import OrderLine from './OrderLine';
import ChangeQuantityModal from './ChangeQuantityModal'; 

class Basket extends React.Component {
    constructor(props){
        super(props);
        this.state = { selectedOrder: null };
        this.changeQuantityModal = this.changeQuantityModal.bind(this);
    }    

    changeQuantityModal(order) {
        this.setState({ selectedOrder: order });
    }

    render() {
        let { basket, changeQuantity, cancelBasket } = this.props;
        let { selectedOrder } = this.state;
        let orderLines = [];

        if(!basket.id) {
            return null;
        }

        if(basket) {
            orderLines = basket.orderLines.map(p => 
                OrderLine(p, () => this.changeQuantityModal(p))
            );
        }
        
        return (
            <div>
                <ChangeQuantityModal
                    order={selectedOrder}
                    changeQuantity={changeQuantity}
                    show={selectedOrder !== null}
                    onClose={() => this.changeQuantityModal(null)}/>

                <Table striped hover>
                    <thead>
                        <tr>
                            <th className='col-md-6'>Name</th>
                            <th className='col-md-2'>Price</th>
                            <th className='col-md-2'>Qty</th>
                            <th className='col-md-2'></th>
                        </tr>
                    </thead>
                    <tbody>{orderLines}</tbody>
                    <tfoot>
                        <tr>
                            <td colSpan='4' className='text-right'>
                                <Button>Check Out</Button>
                                <Button 
                                    bsStyle='danger' 
                                    onClick={() => cancelBasket(basket.id)}>Cancel</Button>
                            </td>
                        </tr>
                    </tfoot>
                </Table>
            </div>
        );
    }
}

export default Basket;